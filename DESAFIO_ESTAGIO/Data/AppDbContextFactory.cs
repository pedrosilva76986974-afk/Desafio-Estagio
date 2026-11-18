using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DESAFIO_ESTAGIO.Data;

// Allows generating scripts and future migrations without JWT/bootstrap secrets.
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Server=localhost;Database=ImdbChallenge;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        return new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connection).Options);
    }
}
