using Microsoft.AspNetCore.Identity;

namespace DESAFIO_ESTAGIO.Domain;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    public bool IsAdministrator { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
