namespace LabPat.Domain.Entities;

public abstract class EntityBase
{
    public int Id { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
