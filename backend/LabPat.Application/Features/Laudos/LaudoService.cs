using LabPat.Application.Common;
using LabPat.Domain.Entities;
using LabPat.Domain.Enums;
using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.Laudos;

public class LaudoService(
    ILaudoRepository laudoRepository,
    ISolicitacaoRepository solicitacaoRepository,
    ITipoExameRepository tipoExameRepository,
    IInsumoRepository insumoRepository,
    IUsuarioRepository usuarioRepository,
    ICurrentUser currentUser,
    IPdfGenerator pdfGenerator,
    IUnitOfWork unitOfWork) : ILaudoService
{
    public async Task<LaudoDto> CreateAsync(CreateLaudoInput input)
    {
        var solicitacao = await solicitacaoRepository.GetByIdComDetalhesAsync(input.SolicitacaoId)
            ?? throw new InvalidOperationException("Solicitação não encontrada.");

        if (solicitacao.Laudo is not null)
            throw new InvalidOperationException("Já existe um laudo para esta solicitação.");

        var laudo = new Laudo
        {
            SolicitacaoId = input.SolicitacaoId,
            Conteudo = input.Conteudo,
            EmitidoPorId = currentUser.Id,
            EmitidoEm = DateTime.UtcNow
        };

        await laudoRepository.AddAsync(laudo);

        var statusAnterior = solicitacao.Status;
        solicitacao.Status = StatusSolicitacao.Concluido;
        solicitacao.DataConclusao = DateTime.UtcNow;
        solicitacao.Historico.Add(new HistoricoStatus
        {
            SolicitacaoId = solicitacao.Id,
            StatusAnterior = statusAnterior,
            StatusNovo = StatusSolicitacao.Concluido,
            AlteradoPorId = currentUser.Id,
            Observacao = "Laudo emitido."
        });

        await DescontarEstoqueAsync(solicitacao.TipoExameId);
        solicitacaoRepository.Update(solicitacao);
        await unitOfWork.CommitAsync();

        var patologista = await usuarioRepository.GetByIdAsync(currentUser.Id);
        return ToDto(laudo, solicitacao, patologista?.Nome ?? string.Empty);
    }

    public async Task<LaudoDto?> GetByIdAsync(int id)
    {
        var laudo = await laudoRepository.GetByIdComDetalhesAsync(id);
        return laudo is null ? null : ToDto(laudo);
    }

    public async Task<LaudoDto?> GetBySolicitacaoAsync(int solicitacaoId)
    {
        var laudo = await laudoRepository.GetBySolicitacaoIdAsync(solicitacaoId);
        return laudo is null ? null : ToDto(laudo);
    }

    public async Task<byte[]?> GerarPdfAsync(int id)
    {
        var laudo = await laudoRepository.GetByIdComDetalhesAsync(id);
        if (laudo is null) return null;

        var s = laudo.Solicitacao!;
        var paciente = s.Paciente!;
        var tutor = paciente.Tutor!;
        var vet = s.VetSolicitante!;

        var idade = paciente.IdadeAnos.HasValue
            ? $"{paciente.IdadeAnos} anos{(paciente.IdadeMeses.HasValue ? $" e {paciente.IdadeMeses} meses" : "")}"
            : paciente.IdadeMeses.HasValue ? $"{paciente.IdadeMeses} meses" : null;

        var data = new LaudoPdfData(
            CodigoPublico: s.CodigoPublico,
            TipoExame: s.TipoExame!.Nome,
            PacienteNome: paciente.Nome,
            Especie: paciente.Especie,
            Raca: paciente.Raca,
            Sexo: paciente.Sexo.ToString(),
            Idade: idade,
            Peso: paciente.PesoKg.HasValue ? $"{paciente.PesoKg:F2} kg" : null,
            TutorNome: tutor.Nome,
            VetSolicitanteNome: vet.Nome,
            CrmvNumero: vet.CrmvNumero,
            CrmvEstado: vet.CrmvEstado,
            DataSolicitacao: s.CriadoEm,
            DataEmissao: laudo.EmitidoEm,
            Conteudo: laudo.Conteudo,
            PatologistaNome: laudo.EmitidoPor?.Nome ?? string.Empty);

        return pdfGenerator.GerarLaudo(data);
    }

    private async Task DescontarEstoqueAsync(int tipoExameId)
    {
        var tipo = await tipoExameRepository.GetByIdComDetalhesAsync(tipoExameId);
        if (tipo is null) return;

        foreach (var ei in tipo.ExameInsumos)
        {
            var insumo = await insumoRepository.GetByIdAsync(ei.InsumoId);
            if (insumo is null) continue;
            insumo.QuantidadeAtual = Math.Max(0, insumo.QuantidadeAtual - ei.QuantidadeConsumida);
            insumoRepository.Update(insumo);
        }
    }

    private static LaudoDto ToDto(Laudo l) =>
        new(l.Id, l.SolicitacaoId,
            l.Solicitacao?.CodigoPublico ?? string.Empty,
            l.Solicitacao?.TipoExame?.Nome ?? string.Empty,
            l.Solicitacao?.Paciente?.Nome ?? string.Empty,
            l.Conteudo,
            l.EmitidoPor?.Nome ?? string.Empty,
            l.EmitidoEm);

    private static LaudoDto ToDto(Laudo l, Solicitacao s, string patologistaNome) =>
        new(l.Id, l.SolicitacaoId,
            s.CodigoPublico,
            s.TipoExame?.Nome ?? string.Empty,
            s.Paciente?.Nome ?? string.Empty,
            l.Conteudo,
            patologistaNome,
            l.EmitidoEm);
}
