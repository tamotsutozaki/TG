namespace LabPat.Application.Features.Gemini;

public record ExtrairSolicitacaoDto(
    string? VetNome,
    string? VetCrmvNumero,
    string? VetCrmvEstado,
    string? VetEmail,
    string? VetTelefone,
    string? TutorNome,
    string? TutorTelefone,
    string? TutorEmail,
    string? PacienteNome,
    string? Especie,
    string? Raca,
    string? Sexo,
    string? Idade,
    string? Peso,
    string? TipoExame,
    string? DescricaoClinica);
