using LabPat.Domain.Enums;

namespace LabPat.Application.Features.Vinculos;

public record SolicitarVinculoInput(int LaboratorioId);

// StatusVinculo: situação do vínculo do veterinário logado com o laboratório (nulo se não houver)
public record LaboratorioBuscaDto(
    int Id,
    string Nome,
    string Cidade,
    string Uf,
    StatusVinculo? StatusVinculo);

public record MeuVinculoDto(
    int Id,
    int LaboratorioId,
    string LaboratorioNome,
    string Cidade,
    string Uf,
    StatusVinculo Status,
    DateTime SolicitadoEm,
    DateTime? RespondidoEm);
