using LabPat.Application.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace LabPat.Api.Middleware;

/// <summary>
/// Converte as exceções de regra de negócio em respostas HTTP com { message }.
/// Demais exceções seguem para o tratamento padrão (500).
/// </summary>
public class ExcecaoHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            RegraNegocioException => StatusCodes.Status400BadRequest,
            AcessoNegadoException => StatusCodes.Status403Forbidden,
            ConflitoException => StatusCodes.Status409Conflict,
            _ => 0
        };

        if (status == 0) return false;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(new { message = exception.Message }, cancellationToken);
        return true;
    }
}
