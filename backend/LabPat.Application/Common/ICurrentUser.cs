namespace LabPat.Application.Common;

public interface ICurrentUser
{
    int Id { get; }
    string? Papel { get; }

    // Preenchido apenas para a conta de um laboratório
    int? LaboratorioId { get; }
}
