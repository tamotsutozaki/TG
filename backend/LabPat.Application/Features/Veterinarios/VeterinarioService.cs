using LabPat.Domain.Enums;
using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.Veterinarios;

public class VeterinarioService(
    IUsuarioRepository usuarioRepository,
    IUnitOfWork unitOfWork) : IVeterinarioService
{
    public async Task<IEnumerable<VeterinarioContaDto>> ListarAsync()
    {
        var veterinarios = await usuarioRepository.GetVeterinariosAsync();
        return veterinarios.Select(u => new VeterinarioContaDto(
            u.Id,
            u.Nome,
            u.Email,
            u.VetSolicitante?.CrmvNumero ?? string.Empty,
            u.VetSolicitante?.CrmvEstado ?? string.Empty,
            u.VetSolicitante?.Telefone,
            u.StatusConta.ToString(),
            u.CriadoEm));
    }

    public Task<bool> AprovarAsync(int id) => AlterarStatusAsync(id, StatusConta.Aprovada);

    public Task<bool> RecusarAsync(int id) => AlterarStatusAsync(id, StatusConta.Recusada);

    private async Task<bool> AlterarStatusAsync(int id, StatusConta status)
    {
        var usuario = await usuarioRepository.GetByIdAsync(id);
        if (usuario is null || usuario.Papel != PapelUsuario.Veterinario)
            return false;

        usuario.StatusConta = status;
        usuarioRepository.Update(usuario);
        await unitOfWork.CommitAsync();
        return true;
    }
}
