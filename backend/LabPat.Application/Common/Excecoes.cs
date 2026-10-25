namespace LabPat.Application.Common;

// Exceções de regra de negócio, convertidas em respostas HTTP pelo handler da API

/// <summary>Dados inválidos ou operação não permitida no estado atual (400).</summary>
public class RegraNegocioException(string message) : Exception(message);

/// <summary>Usuário autenticado sem permissão para o recurso (403).</summary>
public class AcessoNegadoException(string message) : Exception(message);

/// <summary>Conflito com dados já existentes (409).</summary>
public class ConflitoException(string message) : Exception(message);
