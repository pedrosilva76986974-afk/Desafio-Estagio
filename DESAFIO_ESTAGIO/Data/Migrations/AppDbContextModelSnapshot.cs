using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DESAFIO_ESTAGIO.Data.Migrations;

[DbContext(typeof(AppDbContext))]
public sealed class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder) => InitialModel.Build(modelBuilder);
}
