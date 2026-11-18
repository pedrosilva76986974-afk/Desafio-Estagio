using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DESAFIO_ESTAGIO.Infrastructure;

public sealed class BearerSecurityFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        if (metadata.OfType<IAllowAnonymous>().Any() || !metadata.OfType<IAuthorizeData>().Any()) return;
        operation.Security = [new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = []
        }];
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Token ausente, inválido ou conta excluída." });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Perfil sem permissão." });
    }
}
