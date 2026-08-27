using LabPat.Application.Common;
using LabPat.Domain.Entities;
using LabPat.Domain.Enums;
using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.Solicitacoes;

public class SolicitacaoService(
    ISolicitacaoRepository solicitacaoRepository,
    IVetSolicitanteRepository vetRepository,
    ITutorRepository tutorRepository,
    IPacienteRepository pacienteRepository,
    ITipoExameRepository tipoExameRepository,
    IInsumoRepository insumoRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : ISolicitacaoService
{
    public async Task<IEnumerable<SolicitacaoDto>> GetAllAsync()
    {
        var solicitacoes = await solicitacaoRepository.GetAllComResumoAsync();
        return solicitacoes.Select(ToDto);
    }

    public async Task<SolicitacaoDetalhadaDto?> GetByIdAsync(int id)
    {
        var s = await solicitacaoRepository.GetByIdComDetalhesAsync(id);
        return s is null ? null : ToDetalhadoDto(s);
    }

    public async Task<SolicitacaoDto> CreateAsync(CreateSolicitacaoInput input)
    {
        var vet = await BuscarOuCriarVetAsync(input.VetSolicitante);
        var tutor = await BuscarOuCriarTutorAsync(input.Tutor);
        var paciente = await BuscarOuCriarPacienteAsync(input.Paciente, tutor.Id);

        var codigo = await GerarCodigoUnicoAsync();
        var tipoExame = await tipoExameRepository.GetByIdAsync(input.TipoExameId);

        var solicitacao = new Solicitacao
        {
            CodigoPublico = codigo,
            PacienteId = paciente.Id,
            VetSolicitanteId = vet.Id,
            TipoExameId = input.TipoExameId,
            Status = StatusSolicitacao.Solicitado,
            DescricaoClinica = input.DescricaoClinica,
            MetodoEntrada = input.MetodoEntrada,
            ArquivoEntradaUrl = input.ArquivoEntradaUrl,
            DataEstimadaConclusao = tipoExame is not null
                ? DateTime.UtcNow.AddDays(tipoExame.PrazoEstimadoDias)
                : null
        };

        await solicitacaoRepository.AddAsync(solicitacao);
        await unitOfWork.CommitAsync();

        return ToDto(solicitacao, tipoExame?.Nome ?? string.Empty, paciente.Nome, vet.Nome);
    }

    public async Task<SolicitacaoDetalhadaDto?> UpdateStatusAsync(int id, UpdateStatusInput input)
    {
        var solicitacao = await solicitacaoRepository.GetByIdComDetalhesAsync(id);
        if (solicitacao is null) return null;

        var historico = new HistoricoStatus
        {
            SolicitacaoId = solicitacao.Id,
            StatusAnterior = solicitacao.Status,
            StatusNovo = input.NovoStatus,
            AlteradoPorId = currentUser.Id,
            Observacao = input.Observacao
        };

        solicitacao.Status = input.NovoStatus;
        solicitacao.Historico.Add(historico);

        if (input.NovoStatus == StatusSolicitacao.Concluido)
        {
            solicitacao.DataConclusao = DateTime.UtcNow;
            await DescontarEstoqueAsync(solicitacao.TipoExameId);
        }

        solicitacaoRepository.Update(solicitacao);
        await unitOfWork.CommitAsync();

        return ToDetalhadoDto(solicitacao);
    }

    public async Task<ConsultaPublicaDto?> GetByCodigoPublicoAsync(string codigo)
    {
        var s = await solicitacaoRepository.GetByCodigoPublicoAsync(codigo);
        if (s is null) return null;

        return new ConsultaPublicaDto(
            s.CodigoPublico,
            s.Status.ToString(),
            s.TipoExame?.Nome ?? string.Empty,
            s.Paciente?.Nome ?? string.Empty,
            s.CriadoEm,
            s.DataEstimadaConclusao);
    }

    // --- helpers ---

    private async Task<VetSolicitante> BuscarOuCriarVetAsync(VetSolicitanteInput input)
    {
        var vet = await vetRepository.GetByCrmvAsync(input.CrmvNumero, input.CrmvEstado);
        if (vet is not null) return vet;

        vet = new VetSolicitante
        {
            Nome = input.Nome,
            CrmvNumero = input.CrmvNumero,
            CrmvEstado = input.CrmvEstado,
            Email = input.Email,
            Telefone = input.Telefone
        };
        await vetRepository.AddAsync(vet);
        return vet;
    }

    private async Task<Tutor> BuscarOuCriarTutorAsync(TutorInput input)
    {
        var tutor = await tutorRepository.GetByTelefoneAsync(input.Telefone);
        if (tutor is not null) return tutor;

        tutor = new Tutor
        {
            Nome = input.Nome,
            Telefone = input.Telefone,
            Email = input.Email
        };
        await tutorRepository.AddAsync(tutor);
        return tutor;
    }

    private async Task<Paciente> BuscarOuCriarPacienteAsync(PacienteInput input, int tutorId)
    {
        var paciente = await pacienteRepository.GetByNomeETutorAsync(input.Nome, tutorId);
        if (paciente is not null) return paciente;

        paciente = new Paciente
        {
            Nome = input.Nome,
            Especie = input.Especie,
            Raca = input.Raca,
            Sexo = input.Sexo,
            IdadeAnos = input.IdadeAnos,
            IdadeMeses = input.IdadeMeses,
            PesoKg = input.PesoKg,
            TutorId = tutorId
        };
        await pacienteRepository.AddAsync(paciente);
        return paciente;
    }

    private async Task<string> GerarCodigoUnicoAsync()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        string codigo;

        do
        {
            codigo = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        while (await solicitacaoRepository.CodigoPublicoExisteAsync(codigo));

        return codigo;
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

    private static SolicitacaoDto ToDto(Solicitacao s) =>
        new(s.Id, s.CodigoPublico, s.Status,
            s.TipoExame?.Nome ?? string.Empty,
            s.Paciente?.Nome ?? string.Empty,
            s.VetSolicitante?.Nome ?? string.Empty,
            s.CriadoEm, s.DataEstimadaConclusao);

    private static SolicitacaoDto ToDto(Solicitacao s, string tipoNome, string pacienteNome, string vetNome) =>
        new(s.Id, s.CodigoPublico, s.Status, tipoNome, pacienteNome, vetNome,
            s.CriadoEm, s.DataEstimadaConclusao);

    private static SolicitacaoDetalhadaDto ToDetalhadoDto(Solicitacao s) =>
        new(s.Id, s.CodigoPublico, s.Status, s.MetodoEntrada,
            s.DescricaoClinica, s.ObservacoesInternas, s.ArquivoEntradaUrl,
            s.CriadoEm, s.DataEstimadaConclusao, s.DataConclusao,
            new TipoExameResumoDto(s.TipoExame!.Id, s.TipoExame.Nome, s.TipoExame.PrazoEstimadoDias),
            new PacienteResumoDto(s.Paciente!.Id, s.Paciente.Nome, s.Paciente.Especie,
                s.Paciente.Raca, s.Paciente.Sexo, s.Paciente.IdadeAnos,
                s.Paciente.IdadeMeses, s.Paciente.PesoKg),
            new TutorResumoDto(s.Paciente.Tutor!.Id, s.Paciente.Tutor.Nome,
                s.Paciente.Tutor.Telefone, s.Paciente.Tutor.Email),
            new VetSolicitanteResumoDto(s.VetSolicitante!.Id, s.VetSolicitante.Nome,
                s.VetSolicitante.CrmvNumero, s.VetSolicitante.CrmvEstado,
                s.VetSolicitante.Email, s.VetSolicitante.Telefone),
            s.Historico.OrderBy(h => h.AlteradoEm)
                .Select(h => new HistoricoStatusDto(h.StatusAnterior, h.StatusNovo, h.AlteradoEm, h.Observacao)));
}
