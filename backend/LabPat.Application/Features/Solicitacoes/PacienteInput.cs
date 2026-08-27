using LabPat.Domain.Enums;

namespace LabPat.Application.Features.Solicitacoes;

public record PacienteInput(
    string Nome,
    string Especie,
    string? Raca,
    SexoPaciente Sexo,
    int? IdadeAnos,
    int? IdadeMeses,
    decimal? PesoKg);
