namespace LabPat.Application.Features.Veterinarios;

// Gestão, pelo laboratório logado, dos veterinários vinculados a ele
public interface IVeterinarioService
{
    Task<IEnumerable<VinculoVeterinarioDto>> ListarAsync();
    Task<VinculoVeterinarioDto?> AprovarAsync(int vinculoId);
    Task<VinculoVeterinarioDto?> RecusarAsync(int vinculoId);
    Task<VinculoVeterinarioDto?> DesfazerAsync(int vinculoId);
}
