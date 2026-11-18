using DESAFIO_ESTAGIO.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DESAFIO_ESTAGIO.Data;

public static class AdministratorBootstrap
{
    public static async Task RunAsync(IServiceProvider services, IConfiguration configuration)
    {
        var email = configuration["BootstrapAdmin:Email"];
        var password = configuration["BootstrapAdmin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Configure BootstrapAdmin__Email e BootstrapAdmin__Password para criar o administrador inicial.");
        var manager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var db = services.GetRequiredService<AppDbContext>();
        var normalized = manager.NormalizeEmail(email.Trim());
        var existing = await db.Users.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.NormalizedEmail == normalized);
        if (existing is not null)
        {
            if (existing.IsDeleted || !existing.IsAdministrator)
                throw new InvalidOperationException("A conta de bootstrap já existe e não é um administrador ativo.");
            return;
        }
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(), Name = "Administrador inicial", Email = email.Trim(), UserName = email.Trim(),
            IsAdministrator = true, LockoutEnabled = true
        };
        var result = await manager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException("Falha ao criar administrador: " + string.Join(" ", result.Errors.Select(x => x.Description)));
    }
}
