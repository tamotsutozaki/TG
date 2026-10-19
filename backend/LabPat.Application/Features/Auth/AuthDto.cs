namespace LabPat.Application.Features.Auth;

public record AuthDto(string Token, string Nome, string Email, string Papel);

public enum LoginStatus
{
    Sucesso,
    CredenciaisInvalidas,
    ContaPendente,
    ContaRecusada
}

public record LoginResultado(LoginStatus Status, AuthDto? Auth = null);

public enum RegistroStatus
{
    Sucesso,
    EmailJaCadastrado,
    CrmvInvalido
}

public record RegistroResultado(RegistroStatus Status);
