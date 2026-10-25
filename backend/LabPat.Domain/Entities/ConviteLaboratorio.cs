namespace LabPat.Domain.Entities;

public class ConviteLaboratorio : EntityBase
{
    // Apenas o hash SHA-256 do token é armazenado; o token em si só é exibido na criação
    public string TokenHash { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime ExpiraEm { get; set; }
    public DateTime? UsadoEm { get; set; }
    public DateTime? RevogadoEm { get; set; }

    public int CriadoPorId { get; set; }
    public Usuario CriadoPor { get; set; } = null!;

    // Laboratório cadastrado com este convite
    public int? LaboratorioId { get; set; }
    public Laboratorio? Laboratorio { get; set; }

    // Controle de concorrência: impede que o mesmo convite seja usado duas vezes ao mesmo tempo
    public byte[] Versao { get; set; } = [];
}
