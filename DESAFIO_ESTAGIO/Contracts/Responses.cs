using DESAFIO_ESTAGIO.Domain;

namespace DESAFIO_ESTAGIO.Contracts;

public sealed record UserResponse(Guid Id, string Name, string Email, bool IsAdministrator)
{
    public static UserResponse From(ApplicationUser user) => new(user.Id, user.Name, user.Email!, user.IsAdministrator);
}

public sealed record TokenResponse(string AccessToken, string TokenType, DateTimeOffset ExpiresAt);
public sealed record ListResponse<T>(IReadOnlyList<T> Items, int Total, int? Page, int? PageSize);
public sealed record MovieSummary(Guid Id, string Title, string Director, string Genre, int ReleaseYear, int VoteCount, double AverageScore);
public sealed record MovieDetails(Guid Id, string Title, string Synopsis, string Director, string Genre,
    int ReleaseYear, IReadOnlyList<string> Actors, int VoteCount, double AverageScore);
