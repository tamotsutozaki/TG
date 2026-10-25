using LabPat.Application.Common;

namespace LabPat.Infrastructure.Identity;

public class TenantContext(ICurrentUser currentUser) : ITenantContext
{
    private int? laboratorioDefinido;

    public int? LaboratorioId => laboratorioDefinido ?? currentUser.LaboratorioId;

    public void Definir(int laboratorioId)
    {
        // A conta de um laboratório nunca pode operar no escopo de outro
        if (currentUser.LaboratorioId is int proprio && proprio != laboratorioId)
            throw new InvalidOperationException("A conta de um laboratório não pode acessar outro laboratório.");

        laboratorioDefinido = laboratorioId;
    }
}
