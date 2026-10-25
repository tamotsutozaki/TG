namespace LabPat.Application.Common;

/// <summary>
/// Laboratório em cujo escopo a requisição atual lê e grava dados.
/// Por padrão é o laboratório do usuário logado; para veterinários e anônimos é nulo,
/// e nesse caso as consultas filtradas não retornam nada.
/// </summary>
public interface ITenantContext
{
    int? LaboratorioId { get; }

    /// <summary>
    /// Entra no escopo de um laboratório durante a requisição (ex.: veterinário com vínculo
    /// aprovado criando uma solicitação). Só deve ser chamado depois de checar a permissão.
    /// </summary>
    void Definir(int laboratorioId);
}
