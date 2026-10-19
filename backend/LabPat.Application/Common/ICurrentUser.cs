namespace LabPat.Application.Common;

public interface ICurrentUser
{
    int Id { get; }
    string? Papel { get; }
    int? VetSolicitanteId { get; }
}
