using Microsoft.EntityFrameworkCore;

namespace DESAFIO_ESTAGIO.Data.Migrations;

// Frozen model for InitialCreate. Keep unchanged when adding subsequent migrations.
internal static class InitialModel
{
    internal static void Build(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.25").HasAnnotation("Relational:MaxIdentifierLength", 128);
        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("DESAFIO_ESTAGIO.Domain.ApplicationUser", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
            b.Property<string>("Name").IsRequired().HasMaxLength(150).HasColumnType("nvarchar(150)");
            b.Property<bool>("IsAdministrator").HasColumnType("bit");
            b.Property<bool>("IsDeleted").HasColumnType("bit");
            b.Property<DateTimeOffset?>("DeletedAt").HasColumnType("datetimeoffset");
            b.Property<string>("UserName").HasMaxLength(256).HasColumnType("nvarchar(256)");
            b.Property<string>("NormalizedUserName").HasMaxLength(256).HasColumnType("nvarchar(256)");
            b.Property<string>("Email").IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");
            b.Property<string>("NormalizedEmail").IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");
            b.Property<bool>("EmailConfirmed").HasColumnType("bit");
            b.Property<string>("PasswordHash").HasColumnType("nvarchar(max)");
            b.Property<string>("SecurityStamp").HasColumnType("nvarchar(max)");
            b.Property<string>("ConcurrencyStamp").IsConcurrencyToken().HasColumnType("nvarchar(max)");
            b.Property<string>("PhoneNumber").HasColumnType("nvarchar(max)");
            b.Property<bool>("PhoneNumberConfirmed").HasColumnType("bit");
            b.Property<bool>("TwoFactorEnabled").HasColumnType("bit");
            b.Property<DateTimeOffset?>("LockoutEnd").HasColumnType("datetimeoffset");
            b.Property<bool>("LockoutEnabled").HasColumnType("bit");
            b.Property<int>("AccessFailedCount").HasColumnType("int");
            b.HasKey("Id");
            b.HasIndex("NormalizedEmail").IsUnique().HasDatabaseName("EmailIndex");
            b.HasIndex("NormalizedUserName").IsUnique().HasDatabaseName("UserNameIndex").HasFilter("[NormalizedUserName] IS NOT NULL");
            b.ToTable("AspNetUsers");
        });
        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
            b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
            b.Property<string>("ClaimType").HasColumnType("nvarchar(max)");
            b.Property<string>("ClaimValue").HasColumnType("nvarchar(max)");
            b.HasKey("Id");
            b.HasIndex("UserId");
            b.ToTable("AspNetUserClaims");
        });
        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
        {
            b.Property<string>("LoginProvider").HasColumnType("nvarchar(450)");
            b.Property<string>("ProviderKey").HasColumnType("nvarchar(450)");
            b.Property<string>("ProviderDisplayName").HasColumnType("nvarchar(max)");
            b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
            b.HasKey("LoginProvider", "ProviderKey");
            b.HasIndex("UserId");
            b.ToTable("AspNetUserLogins");
        });
        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
        {
            b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
            b.Property<string>("LoginProvider").HasColumnType("nvarchar(450)");
            b.Property<string>("Name").HasColumnType("nvarchar(450)");
            b.Property<string>("Value").HasColumnType("nvarchar(max)");
            b.HasKey("UserId", "LoginProvider", "Name");
            b.ToTable("AspNetUserTokens");
        });
        modelBuilder.Entity("DESAFIO_ESTAGIO.Domain.Movie", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
            b.Property<string>("Title").IsRequired().HasMaxLength(200).HasColumnType("nvarchar(200)");
            b.Property<string>("Synopsis").IsRequired().HasMaxLength(4000).HasColumnType("nvarchar(4000)");
            b.Property<string>("Director").IsRequired().HasMaxLength(150).HasColumnType("nvarchar(150)");
            b.Property<string>("Genre").IsRequired().HasMaxLength(80).HasColumnType("nvarchar(80)");
            b.Property<int>("ReleaseYear").HasColumnType("int");
            b.HasKey("Id");
            b.HasIndex("Title");
            b.ToTable("Movies", t => t.HasCheckConstraint("CK_Movies_ReleaseYear", "[ReleaseYear] >= 1888 AND [ReleaseYear] <= 2100"));
        });
        modelBuilder.Entity("DESAFIO_ESTAGIO.Domain.MovieActor", b =>
        {
            b.Property<Guid>("MovieId").HasColumnType("uniqueidentifier");
            b.Property<string>("Name").HasMaxLength(150).HasColumnType("nvarchar(150)");
            b.HasKey("MovieId", "Name");
            b.ToTable("MovieActors");
        });
        modelBuilder.Entity("DESAFIO_ESTAGIO.Domain.Vote", b =>
        {
            b.Property<Guid>("MovieId").HasColumnType("uniqueidentifier");
            b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
            b.Property<int>("Score").HasColumnType("int");
            b.HasKey("MovieId", "UserId");
            b.HasIndex("UserId");
            b.ToTable("Votes", t => t.HasCheckConstraint("CK_Votes_Score", "[Score] >= 0 AND [Score] <= 4"));
        });
        foreach (var entity in new[]
        {
            "Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>",
            "Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>",
            "Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>"
        })
            modelBuilder.Entity(entity, b => b.HasOne("DESAFIO_ESTAGIO.Domain.ApplicationUser", null)
                .WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired());
        modelBuilder.Entity("DESAFIO_ESTAGIO.Domain.MovieActor", b => b.HasOne("DESAFIO_ESTAGIO.Domain.Movie", null)
            .WithMany("Actors").HasForeignKey("MovieId").OnDelete(DeleteBehavior.Cascade).IsRequired());
        modelBuilder.Entity("DESAFIO_ESTAGIO.Domain.Vote", b =>
        {
            b.HasOne("DESAFIO_ESTAGIO.Domain.Movie", null).WithMany("Votes").HasForeignKey("MovieId").OnDelete(DeleteBehavior.Cascade).IsRequired();
            b.HasOne("DESAFIO_ESTAGIO.Domain.ApplicationUser", null).WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Restrict).IsRequired();
        });
        modelBuilder.Entity("DESAFIO_ESTAGIO.Domain.Movie", b => { b.Navigation("Actors"); b.Navigation("Votes"); });
    }
}
