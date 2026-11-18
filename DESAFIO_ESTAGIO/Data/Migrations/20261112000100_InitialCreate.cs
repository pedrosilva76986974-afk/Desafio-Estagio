using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DESAFIO_ESTAGIO.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261112000100_InitialCreate")]
public sealed class InitialCreate : Migration
{
    protected override void BuildTargetModel(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder) => InitialModel.Build(modelBuilder);

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("AspNetUsers", columns: table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
            IsAdministrator = table.Column<bool>(type: "bit", nullable: false),
            IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
            PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
            SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
            TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
            LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
            AccessFailedCount = table.Column<int>(type: "int", nullable: false)
        }, constraints: table => table.PrimaryKey("PK_AspNetUsers", x => x.Id));

        migrationBuilder.CreateTable("Movies", columns: table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            Synopsis = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
            Director = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
            Genre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
            ReleaseYear = table.Column<int>(type: "int", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_Movies", x => x.Id);
            table.CheckConstraint("CK_Movies_ReleaseYear", "[ReleaseYear] >= 1888 AND [ReleaseYear] <= 2100");
        });

        migrationBuilder.CreateTable("AspNetUserClaims", columns: table => new
        {
            Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
            table.ForeignKey("FK_AspNetUserClaims_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("AspNetUserLogins", columns: table => new
        {
            LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
            table.ForeignKey("FK_AspNetUserLogins_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("AspNetUserTokens", columns: table => new
        {
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
            table.ForeignKey("FK_AspNetUserTokens_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("MovieActors", columns: table => new
        {
            MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_MovieActors", x => new { x.MovieId, x.Name });
            table.ForeignKey("FK_MovieActors_Movies_MovieId", x => x.MovieId, "Movies", "Id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("Votes", columns: table => new
        {
            MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Score = table.Column<int>(type: "int", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_Votes", x => new { x.MovieId, x.UserId });
            table.CheckConstraint("CK_Votes_Score", "[Score] >= 0 AND [Score] <= 4");
            table.ForeignKey("FK_Votes_Movies_MovieId", x => x.MovieId, "Movies", "Id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("FK_Votes_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Restrict);
        });

        migrationBuilder.CreateIndex("EmailIndex", "AspNetUsers", "NormalizedEmail", unique: true);
        migrationBuilder.CreateIndex("UserNameIndex", "AspNetUsers", "NormalizedUserName", unique: true, filter: "[NormalizedUserName] IS NOT NULL");
        migrationBuilder.CreateIndex("IX_AspNetUserClaims_UserId", "AspNetUserClaims", "UserId");
        migrationBuilder.CreateIndex("IX_AspNetUserLogins_UserId", "AspNetUserLogins", "UserId");
        migrationBuilder.CreateIndex("IX_Movies_Title", "Movies", "Title");
        migrationBuilder.CreateIndex("IX_Votes_UserId", "Votes", "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("AspNetUserClaims");
        migrationBuilder.DropTable("AspNetUserLogins");
        migrationBuilder.DropTable("AspNetUserTokens");
        migrationBuilder.DropTable("MovieActors");
        migrationBuilder.DropTable("Votes");
        migrationBuilder.DropTable("Movies");
        migrationBuilder.DropTable("AspNetUsers");
    }
}

