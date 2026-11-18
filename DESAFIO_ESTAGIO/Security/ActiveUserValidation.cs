using System.Security.Claims;
using DESAFIO_ESTAGIO.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace DESAFIO_ESTAGIO.Security;

public static class ActiveUserValidation
{
    public static async Task Validate(TokenValidatedContext context)
    {
        if (!Guid.TryParse(context.Principal?.FindFirstValue("sub"), out var id))
        {
            context.Fail("Token inválido.");
            return;
        }
        var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, context.HttpContext.RequestAborted);
        if (user is null || user.SecurityStamp != context.Principal?.FindFirstValue("security_stamp") ||
            context.Principal?.FindFirstValue("role") != (user.IsAdministrator ? Roles.Administrator : Roles.User))
            context.Fail("Conta ou credencial inválida.");
    }
}
