using System.ComponentModel.DataAnnotations;

namespace DESAFIO_ESTAGIO.Contracts;

public sealed class CreateUserRequest
{
    [Required, StringLength(150, MinimumLength = 2)] public string Name { get; init; } = string.Empty;
    [Required, EmailAddress, StringLength(256)] public string Email { get; init; } = string.Empty;
    [Required, StringLength(128, MinimumLength = 10)] public string Password { get; init; } = string.Empty;
}

public sealed class UpdateUserRequest
{
    [Required, StringLength(150, MinimumLength = 2)] public string Name { get; init; } = string.Empty;
    [Required, EmailAddress, StringLength(256)] public string Email { get; init; } = string.Empty;
}

public sealed class LoginRequest
{
    [Required, EmailAddress, StringLength(256)] public string Email { get; init; } = string.Empty;
    [Required, StringLength(128)] public string Password { get; init; } = string.Empty;
}

public class ListQuery
{
    [Range(1, int.MaxValue)] public int? Page { get; init; }
    [Range(1, 100)] public int PageSize { get; init; } = 20;
}

public enum MovieSort { Votes, Title }

public sealed class MovieQuery : ListQuery
{
    [StringLength(200)] public string? Name { get; init; }
    [StringLength(150)] public string? Director { get; init; }
    [StringLength(80)] public string? Genre { get; init; }
    [StringLength(150)] public string? Actor { get; init; }
    [EnumDataType(typeof(MovieSort))] public MovieSort Sort { get; init; } = MovieSort.Votes;
}

public sealed class CreateMovieRequest : IValidatableObject
{
    [Required, StringLength(200)] public string Title { get; init; } = string.Empty;
    [Required, StringLength(4000)] public string Synopsis { get; init; } = string.Empty;
    [Required, StringLength(150)] public string Director { get; init; } = string.Empty;
    [Required, StringLength(80)] public string Genre { get; init; } = string.Empty;
    [Range(1888, 2100)] public int ReleaseYear { get; init; }
    [Required, MinLength(1), MaxLength(50)] public List<string> Actors { get; init; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Actors is not null && Actors.Any(x => string.IsNullOrWhiteSpace(x) || x.Trim().Length > 150))
            yield return new ValidationResult("Cada ator deve ter entre 1 e 150 caracteres.", [nameof(Actors)]);
    }
}

public sealed class VoteRequest
{
    [Required, Range(0, 4)] public int? Score { get; init; }
}
