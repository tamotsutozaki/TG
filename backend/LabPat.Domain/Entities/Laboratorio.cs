namespace LabPat.Domain.Entities;

public class Laboratorio : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Responsável técnico que assina os laudos emitidos pelo laboratório
    public string ResponsavelTecnicoNome { get; set; } = string.Empty;
    public string ResponsavelTecnicoCrmvNumero { get; set; } = string.Empty;
    public string ResponsavelTecnicoCrmvEstado { get; set; } = string.Empty;

    // Código público do link que o laboratório envia aos veterinários para pedirem vínculo
    public string CodigoVinculo { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public ICollection<Usuario> Usuarios { get; set; } = [];
}
