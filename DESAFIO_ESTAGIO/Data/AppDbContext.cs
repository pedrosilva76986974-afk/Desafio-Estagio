using DESAFIO_ESTAGIO.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DESAFIO_ESTAGIO.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityUserContext<ApplicationUser, Guid>(options)
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<MovieActor> MovieActors => Set<MovieActor>();
    public DbSet<Vote> Votes => Set<Vote>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>(user =>
        {
            user.Property(x => x.Name).HasMaxLength(150).IsRequired();
            user.HasQueryFilter(x => !x.IsDeleted);
            user.Property(x => x.Email).IsRequired();
            user.Property(x => x.NormalizedEmail).IsRequired();
            user.HasIndex(x => x.NormalizedEmail).HasDatabaseName("EmailIndex").IsUnique();
        });
        builder.Entity<Movie>(movie =>
        {
            movie.Property(x => x.Title).HasMaxLength(200).IsRequired();
            movie.Property(x => x.Synopsis).HasMaxLength(4000).IsRequired();
            movie.Property(x => x.Director).HasMaxLength(150).IsRequired();
            movie.Property(x => x.Genre).HasMaxLength(80).IsRequired();
            movie.HasIndex(x => x.Title);
            movie.HasMany(x => x.Actors).WithOne().HasForeignKey(x => x.MovieId);
            movie.HasMany(x => x.Votes).WithOne().HasForeignKey(x => x.MovieId);
            movie.ToTable(t => t.HasCheckConstraint("CK_Movies_ReleaseYear", "[ReleaseYear] >= 1888 AND [ReleaseYear] <= 2100"));
        });
        builder.Entity<MovieActor>(actor =>
        {
            actor.HasKey(x => new { x.MovieId, x.Name });
            actor.Property(x => x.Name).HasMaxLength(150);
        });
        builder.Entity<Vote>(vote =>
        {
            vote.HasKey(x => new { x.MovieId, x.UserId });
            vote.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            vote.ToTable(t => t.HasCheckConstraint("CK_Votes_Score", "[Score] >= 0 AND [Score] <= 4"));
        });
    }
}
