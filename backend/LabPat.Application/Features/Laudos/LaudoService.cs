using LabPat.Application.Common;
using LabPat.Application.Features.Laboratorios;
using LabPat.Application.Features.Vinculos;
using LabPat.Domain.Entities;
using LabPat.Domain.Enums;
using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.Laudos;

public class LaudoService(
    ILaudoRepository laudoRepository,
    ISolicitacaoRepository solicitacaoRepository,
    ITipoExameRepository tipoExameRepository,
    IInsumoRepository insumoRepository,
    ILaboratorioRepository laboratorioRepository,
    IAcessoVeterinarioService acessoVeterinario,
    ICurrentUser currentUser,
    IPdfGenerator pdfGenerator,
    IUnitOfWork unitOfWork) : ILaudoService
{
    public async Task<LaudoDto> CreateAsync(CreateLaudoInput input)
    {
        var solicitacao = await solicitacaoRepository.GetByIdComDetalhesAsync(input.SolicitacaoId)
            ?? throw new RegraNegocioException("Solicitação não encontrada.");

        if (solicitacao.Laudo is not null || await laudoRepository.GetBySolicitacaoIdAsync(solicitacao.Id) is not null)
            throw new RegraNegocioException("Já existe um laudo para esta solicitação.");

        Validacao.Obrigatorio(input.Conteudo, "o conteúdo do laudo");

        var laboratorio = await laboratorioRepository.GetByIdAsync(solicitacao.LaboratorioId)
            ?? throw new RegraNegocioException("Laboratório não encontrado.");

        var laudo = new Laudo
        {
            SolicitacaoId = input.SolicitacaoId,
            Conteudo = input.Conteudo,
            EmitidoPorId = currentUser.Id,
            EmitidoEm = DateTime.UtcNow,
            // Assinatura: responsável técnico do laboratório no momento da emissão
            ResponsavelTecnicoNome = laboratorio.ResponsavelTecnicoNome,
            ResponsavelTecnicoCrmvNumero = laboratorio.ResponsavelTecnicoCrmvNumero,
            ResponsavelTecnicoCrmvEstado = laboratorio.ResponsavelTecnicoCrmvEstado
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

        laudo.Solicitacao = solicitacao;
        return ToDto(laudo);
    }

    public async Task<LaudoDto?> GetByIdAsync(int id)
    {
        var laudo = await ObterLaudoAcessivelAsync(id);
        return laudo is null ? null : ToDto(laudo);
    }

    public async Task<LaudoDto?> GetBySolicitacaoAsync(int solicitacaoId)
    {
        if (!EhVeterinario)
        {
            var doLaboratorio = await laudoRepository.GetBySolicitacaoIdAsync(solicitacaoId);
            return doLaboratorio is null ? null : ToDto(doLaboratorio);
        }

        if (!await VeterinarioPodeVerAsync(solicitacaoId)) return null;
        var laudo = await laudoRepository.GetBySolicitacaoIdAsync(solicitacaoId, ignorarFiltroLaboratorio: true);
        return laudo is null ? null : ToDto(laudo);
    }

    public async Task<byte[]?> GerarPdfAsync(int id)
    {
        var laudo = await ObterLaudoAcessivelAsync(id);
        if (laudo is null) return null;

        var s = laudo.Solicitacao!;
        var laboratorio = s.Laboratorio!;
        var paciente = s.Paciente!;
        var tutor = paciente.Tutor!;
        var vet = s.VetSolicitante!;

        var idade = paciente.IdadeAnos.HasValue
            ? $"{paciente.IdadeAnos} anos{(paciente.IdadeMeses.HasValue ? $" e {paciente.IdadeMeses} meses" : "")}"
            : paciente.IdadeMeses.HasValue ? $"{paciente.IdadeMeses} meses" : null;

        var data = new LaudoPdfData(
            LaboratorioNome: laboratorio.Nome,
            LaboratorioCnpj: CnpjValidacao.Formatar(laboratorio.Cnpj),
            LaboratorioEndereco: $"{laboratorio.Endereco} - {laboratorio.Cidade}/{laboratorio.Uf}",
            LaboratorioTelefone: laboratorio.Telefone,
            LaboratorioEmail: laboratorio.Email,
            CodigoPublico: s.CodigoPublico,
            TipoExame: s.TipoExame!.Nome,
            PacienteNome: paciente.Nome,
            Especie: paciente.Especie,
            Raca: paciente.Raca,
            Sexo: DescreverSexo(paciente.Sexo),
            Idade: idade,
            Peso: paciente.PesoKg.HasValue ? $"{paciente.PesoKg:F2} kg" : null,
            TutorNome: tutor.Nome,
            VetSolicitanteNome: vet.Nome,
            CrmvNumero: vet.CrmvNumero,
            CrmvEstado: vet.CrmvEstado,
            DataSolicitacao: HorarioBrasilia.DeUtc(s.CriadoEm),
            DataEmissao: HorarioBrasilia.DeUtc(laudo.EmitidoEm),
            Conteudo: laudo.Conteudo,
            ResponsavelTecnicoNome: laudo.ResponsavelTecnicoNome,
            ResponsavelTecnicoCrmv: FormatarCrmv(laudo));

        return pdfGenerator.GerarLaudo(data);
    }

    private bool EhVeterinario => currentUser.Papel == PapelUsuario.Veterinario.ToString();

    /// <summary>
    /// Laboratório: consulta filtrada. Veterinário: só se puder ver a solicitação do laudo.
    /// </summary>
    private async Task<Laudo?> ObterLaudoAcessivelAsync(int id)
    {
        if (!EhVeterinario)
            return await laudoRepository.GetByIdComDetalhesAsync(id);

        var laudo = await laudoRepository.GetByIdComDetalhesAsync(id, ignorarFiltroLaboratorio: true);
        return laudo is not null && await VeterinarioPodeVerAsync(laudo.SolicitacaoId) ? laudo : null;
    }

    private async Task<bool> VeterinarioPodeVerAsync(int solicitacaoId)
    {
        var acesso = await acessoVeterinario.ObterAsync();
        return await solicitacaoRepository.GetByIdVisivelAoVeterinarioAsync(solicitacaoId, acesso) is not null;
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

    private static string FormatarCrmv(Laudo l) =>
        $"CRMV-{l.ResponsavelTecnicoCrmvEstado} {l.ResponsavelTecnicoCrmvNumero}";

    private static string DescreverSexo(SexoPaciente sexo) => sexo switch
    {
        SexoPaciente.Macho => "Macho",
        SexoPaciente.Femea => "Fêmea",
        _ => "Não informado"
    };

    private static LaudoDto ToDto(Laudo l) =>
        new(l.Id, l.SolicitacaoId,
            l.Solicitacao?.CodigoPublico ?? string.Empty,
            l.Solicitacao?.TipoExame?.Nome ?? string.Empty,
            l.Solicitacao?.Paciente?.Nome ?? string.Empty,
            l.Conteudo,
            l.ResponsavelTecnicoNome,
            FormatarCrmv(l),
            l.EmitidoEm);
}
