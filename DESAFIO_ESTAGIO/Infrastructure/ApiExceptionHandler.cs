using DESAFIO_ESTAGIO.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DESAFIO_ESTAGIO.Infrastructure;

public sealed class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, detail) = exception switch
        {
            ApiException error => (error.StatusCode, error.Message),
            DbUpdateConcurrencyException => (409, "O registro foi alterado. Atualize os dados e tente novamente."),
            DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } } =>
                (409, "O registro já existe. Atualize os dados e tente novamente."),
            _ => (500, "Ocorreu um erro interno.")
        };
        await Results.Problem(statusCode: status, detail: detail,
            extensions: new Dictionary<string, object?> { ["traceId"] = context.TraceIdentifier }).ExecuteAsync(context);
        return true;
    }
}
