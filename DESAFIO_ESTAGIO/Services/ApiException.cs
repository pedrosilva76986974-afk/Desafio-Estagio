namespace DESAFIO_ESTAGIO.Services;

public sealed class ApiException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
