using LabPat.Application.Common;
using LabPat.Application.Features.Auth;
using LabPat.Application.Features.Vinculos;
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
    IUsuarioRepository usuarioRepository,
    ILaboratorioRepository laboratorioRepository,
    IAcessoVeterinarioService acessoVeterinario,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : ISolicitacaoService
{
    public async Task<IEnumerable<SolicitacaoDto>> GetAllAsync()
    {
        // Laboratório: consulta já filtrada pelo laboratório logado
        if (!EhVeterinario)
            return (await solicitacaoRepository.GetAllComResumoAsync()).Select(ToDto);

        var acesso = await acessoVeterinario.ObterAsync();
        return (await solicitacaoRepository.GetVisiveisAoVeterinarioAsync(acesso)).Select(ToDto);
    }

    public async Task<SolicitacaoDetalhadaDto?> GetByIdAsync(int id)
    {
        if (!EhVeterinario)
        {
            var s = await solicitacaoRepository.GetByIdComDetalhesAsync(id);
            return s is null ? null : ToDetalhadoDto(s);
        }

        var acesso = await acessoVeterinario.ObterAsync();
        var visivel = await solicitacaoRepository.GetByIdVisivelAoVeterinarioAsync(id, acesso);

        // Observações internas são do laboratório; não vão para o veterinário
        return visivel is null ? null : ToDetalhadoDto(visivel) with { ObservacoesInternas = null };
    }

    public async Task<SolicitacaoDto> CreateAsync(CreateSolicitacaoInput input)
    {
        VetSolicitante vet;
        if (EhVeterinario)
        {
            if (input.LaboratorioId is not int laboratorioId)
                throw new RegraNegocioException("Escolha o laboratório que vai realizar o exame.");

            // A partir daqui, as consultas e inclusões ficam no escopo do laboratório escolhido
            await acessoVeterinario.EntrarNoLaboratorioAsync(laboratorioId);
            vet = await ResolverVetDoUsuarioLogadoAsync();
        }
        else
        {
            vet = await ResolverVetDoFormularioAsync(input.VetSolicitante);
        }

        // Consulta filtrada: tipos de exame de outro laboratório não são encontrados
        var tipoExame = await tipoExameRepository.GetByIdAsync(input.TipoExameId);
        if (tipoExame is null || !tipoExame.Ativo)
            throw new RegraNegocioException("Tipo de exame inválido.");

        var tutor = await BuscarOuCriarTutorAsync(input.Tutor);
        var paciente = await BuscarOuCriarPacienteAsync(input.Paciente, tutor);

        var solicitacao = new Solicitacao
        {
            CodigoPublico = await GerarCodigoUnicoAsync(),
            Paciente = paciente,
            VetSolicitante = vet,
            TipoExame = tipoExame,
            CriadoPorId = currentUser.Id,
            Status = StatusSolicitacao.Solicitado,
            DescricaoClinica = input.DescricaoClinica,
            MetodoEntrada = input.MetodoEntrada,
            ArquivoEntradaUrl = input.ArquivoEntradaUrl,
            DataEstimadaConclusao = DateTime.UtcNow.AddDays(tipoExame.PrazoEstimadoDias)
        };

        await solicitacaoRepository.AddAsync(solicitacao);
        await unitOfWork.CommitAsync();

        var laboratorio = await laboratorioRepository.GetByIdAsync(solicitacao.LaboratorioId);
        return ToDto(solicitacao, tipoExame.Nome, paciente.Nome, vet.Nome, laboratorio?.Nome ?? string.Empty);
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
            solicitacao.DataConclusao = DateTime.UtcNow;

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
            s.Laboratorio?.Nome ?? string.Empty,
            s.Status.ToString(),
            s.TipoExame?.Nome ?? string.Empty,
            s.Paciente?.Nome ?? string.Empty,
            s.CriadoEm,
            s.DataEstimadaConclusao);
    }

    // --- helpers ---

    private bool EhVeterinario => currentUser.Papel == PapelUsuario.Veterinario.ToString();

    /// <summary>
    /// Veterinário logado: usa o cadastro que o laboratório já tem com o CRMV dele
    /// ou cria um a partir do perfil da conta (ignora a seção do formulário).
    /// </summary>
    private async Task<VetSolicitante> ResolverVetDoUsuarioLogadoAsync()
    {
        var usuario = await usuarioRepository.GetByIdAsync(currentUser.Id)
            ?? throw new AcessoNegadoException("Conta não encontrada.");

        var vet = await vetRepository.GetByCrmvAsync(usuario.CrmvNumero!, usuario.CrmvEstado!);
        if (vet is not null) return vet;

        vet = new VetSolicitante
        {
            Nome = usuario.Nome,
            CrmvNumero = usuario.CrmvNumero!,
            CrmvEstado = usuario.CrmvEstado!,
            Email = usuario.Email,
            Telefone = usuario.Telefone
        };
        await vetRepository.AddAsync(vet);
        return vet;
    }

    /// <summary>Laboratório logado: busca ou cria o vet a partir dos dados informados.</summary>
    private async Task<VetSolicitante> ResolverVetDoFormularioAsync(VetSolicitanteInput? input)
    {
        if (input is null)
            throw new RegraNegocioException("Dados do veterinário solicitante são obrigatórios.");

        var nome = Validacao.Obrigatorio(input.Nome, "o nome do veterinário solicitante");
        if (!CrmvValidacao.FormatoValido(input.CrmvNumero, input.CrmvEstado))
            throw new RegraNegocioException("CRMV do veterinário solicitante inválido. Verifique o número e a UF.");

        var crmvNumero = input.CrmvNumero.Trim();
        var crmvEstado = input.CrmvEstado.Trim().ToUpperInvariant();

        var vet = await vetRepository.GetByCrmvAsync(crmvNumero, crmvEstado);
        if (vet is not null) return vet;

        vet = new VetSolicitante
        {
            Nome = nome,
            CrmvNumero = crmvNumero,
            CrmvEstado = crmvEstado,
            Email = input.Email,
            Telefone = input.Telefone
        };
        await vetRepository.AddAsync(vet);
        return vet;
    }

    private async Task<Tutor> BuscarOuCriarTutorAsync(TutorInput input)
    {
        var nome = Validacao.Obrigatorio(input.Nome, "o nome do tutor");
        var telefone = Validacao.Obrigatorio(input.Telefone, "o telefone do tutor");

        var tutor = await tutorRepository.GetByTelefoneAsync(telefone);
        if (tutor is not null) return tutor;

        tutor = new Tutor
        {
            Nome = nome,
            Telefone = telefone,
            Email = input.Email
        };
        await tutorRepository.AddAsync(tutor);
        return tutor;
    }

    private async Task<Paciente> BuscarOuCriarPacienteAsync(PacienteInput input, Tutor tutor)
    {
        var nome = Validacao.Obrigatorio(input.Nome, "o nome do paciente");
        var especie = Validacao.Obrigatorio(input.Especie, "a espécie do paciente");

        var paciente = await pacienteRepository.GetByNomeETutorAsync(nome, tutor.Id);
        if (paciente is not null) return paciente;

        paciente = new Paciente
        {
            Nome = nome,
            Especie = especie,
            Raca = input.Raca,
            Sexo = input.Sexo,
            IdadeAnos = input.IdadeAnos,
            IdadeMeses = input.IdadeMeses,
            PesoKg = input.PesoKg,
            Tutor = tutor
        };
        await pacienteRepository.AddAsync(paciente);
        return paciente;
    }

    private async Task<string> GerarCodigoUnicoAsync()
    {
        string codigo;
        do codigo = GeradorCodigo.GerarCodigo(8);
        while (await solicitacaoRepository.CodigoPublicoExisteAsync(codigo));
        return codigo;
    }

    private static SolicitacaoDto ToDto(Solicitacao s) =>
        new(s.Id, s.CodigoPublico, s.Status,
            s.TipoExame?.Nome ?? string.Empty,
            s.Paciente?.Nome ?? string.Empty,
            s.VetSolicitante?.Nome ?? string.Empty,
            s.Laboratorio?.Nome ?? string.Empty,
            s.CriadoEm, s.DataEstimadaConclusao);

    private static SolicitacaoDto ToDto(
        Solicitacao s, string tipoNome, string pacienteNome, string vetNome, string laboratorioNome) =>
        new(s.Id, s.CodigoPublico, s.Status, tipoNome, pacienteNome, vetNome, laboratorioNome,
            s.CriadoEm, s.DataEstimadaConclusao);

    private static SolicitacaoDetalhadaDto ToDetalhadoDto(Solicitacao s) =>
        new(s.Id, s.CodigoPublico, s.Status, s.MetodoEntrada,
            s.DescricaoClinica, s.ObservacoesInternas, s.ArquivoEntradaUrl,
            s.CriadoEm, s.DataEstimadaConclusao, s.DataConclusao,
            s.Laboratorio?.Nome ?? string.Empty,
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
