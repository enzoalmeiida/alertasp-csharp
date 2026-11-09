using Microsoft.AspNetCore.Diagnostics;

namespace Alertasp.Api.Infrastructure;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var isClientError = exception is ArgumentException;
        var statusCode = isClientError ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError;
        var title = isClientError ? "Dados inválidos" : "Erro interno";
        var detail = isClientError ? exception.Message : "Não foi possível concluir a solicitação.";

        logger.LogError(exception, "Falha ao processar a solicitação HTTP.");
        await Results.Problem(statusCode: statusCode, title: title, detail: detail).ExecuteAsync(httpContext);
        return true;
    }
}
