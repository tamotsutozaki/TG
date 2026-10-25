namespace LabPat.Application.Features.Auth;

public record AuthDto(string Token, string Nome, string Email, string Papel);

public enum LoginStatus
{
    Sucesso,
    CredenciaisInvalidas,
    ContaDesativada
}

public record LoginResultado(LoginStatus Status, AuthDto? Auth = null);

public record ConviteInfoDto(string? Descricao, DateTime ExpiraEm);
