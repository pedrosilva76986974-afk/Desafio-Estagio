using System.Data;
using DESAFIO_ESTAGIO.Contracts;
using DESAFIO_ESTAGIO.Data;
using DESAFIO_ESTAGIO.Domain;
using DESAFIO_ESTAGIO.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DESAFIO_ESTAGIO.Services;

public sealed class UserService(AppDbContext db, UserManager<ApplicationUser> manager, ITokenService tokens, TimeProvider clock)
{
    public async Task<UserResponse> CreateAsync(CreateUserRequest request, bool administrator, CancellationToken ct)
    {
        var email = request.Email.Trim();
        await EnsureEmailAvailableAsync(email, null, ct);
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(), Name = CleanName(request.Name), Email = email,
            UserName = email, IsAdministrator = administrator, LockoutEnabled = true
        };
        Check(await manager.CreateAsync(user, request.Password));
        return UserResponse.From(user);
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var user = await manager.FindByEmailAsync(request.Email.Trim());
        if (user is null || await manager.IsLockedOutAsync(user))
            throw new ApiException(401, "E-mail ou senha inválidos.");
        if (!await manager.CheckPasswordAsync(user, request.Password))
        {
            Check(await manager.AccessFailedAsync(user));
            throw new ApiException(401, "E-mail ou senha inválidos.");
        }
        Check(await manager.ResetAccessFailedCountAsync(user));
        return tokens.Create(user);
    }

    public Task<ListResponse<UserResponse>> ListAsync(bool administrators, ListQuery query, CancellationToken ct) =>
        db.Users.AsNoTracking().Where(x => x.IsAdministrator == administrators)
            .OrderBy(x => x.Name).ThenBy(x => x.Id)
            .Select(x => new UserResponse(x.Id, x.Name, x.Email!, x.IsAdministrator))
            .ToListResponseAsync(query, ct);

    public async Task<UserResponse> GetAsync(Guid id, bool administrator, CancellationToken ct) =>
        UserResponse.From(await FindAsync(id, administrator, ct));

    public async Task<UserResponse> UpdateAsync(Guid id, bool administrator, UpdateUserRequest request, CancellationToken ct)
    {
        var user = await FindAsync(id, administrator, ct);
        var email = request.Email.Trim();
        await EnsureEmailAvailableAsync(email, id, ct);
        user.Name = CleanName(request.Name);
        user.Email = email;
        user.UserName = email;
        // Revoke previously issued tokens after account changes.
        user.SecurityStamp = Guid.NewGuid().ToString();
        Check(await manager.UpdateAsync(user));
        return UserResponse.From(user);
    }

    public async Task DeleteAsync(Guid id, bool administrator, CancellationToken ct)
    {
        // Serializable isolation prevents simultaneous deletion of the last two administrators.
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var user = await FindAsync(id, administrator, ct);
        if (administrator && await db.Users.CountAsync(x => x.IsAdministrator, ct) <= 1)
            throw new ApiException(409, "O último administrador ativo não pode ser excluído.");
        user.IsDeleted = true;
        user.DeletedAt = clock.GetUtcNow();
        user.SecurityStamp = Guid.NewGuid().ToString();
        Check(await manager.UpdateAsync(user));
        await transaction.CommitAsync(ct);
    }

    private async Task<ApplicationUser> FindAsync(Guid id, bool administrator, CancellationToken ct) =>
        await db.Users.SingleOrDefaultAsync(x => x.Id == id && x.IsAdministrator == administrator, ct)
        ?? throw new ApiException(404, "Usuário não encontrado.");

    private async Task EnsureEmailAvailableAsync(string email, Guid? exceptId, CancellationToken ct)
    {
        var normalized = manager.NormalizeEmail(email);
        // Deleted accounts retain their e-mail to prevent identity reuse.
        if (await db.Users.IgnoreQueryFilters().AnyAsync(x => x.NormalizedEmail == normalized && x.Id != exceptId, ct))
            throw new ApiException(409, "E-mail indisponível.");
    }

    private static string CleanName(string name) => name.Trim().Length >= 2 ? name.Trim()
        : throw new ApiException(400, "O nome deve ter pelo menos dois caracteres.");

    private static void Check(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            var status = result.Errors.Any(x => x.Code == "ConcurrencyFailure") ? 409 : 400;
            throw new ApiException(status, string.Join(" ", result.Errors.Select(x => x.Description)));
        }
    }
}
