namespace LabPat.Application.Features.Veterinarios;

public interface IVeterinarioService
{
    Task<IEnumerable<VeterinarioContaDto>> ListarAsync();
    Task<bool> AprovarAsync(int id);
    Task<bool> RecusarAsync(int id);
}
