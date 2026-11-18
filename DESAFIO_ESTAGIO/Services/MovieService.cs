using DESAFIO_ESTAGIO.Contracts;
using DESAFIO_ESTAGIO.Data;
using DESAFIO_ESTAGIO.Domain;
using Microsoft.EntityFrameworkCore;

namespace DESAFIO_ESTAGIO.Services;

public sealed class MovieService(AppDbContext db)
{
    public async Task<MovieDetails> CreateAsync(CreateMovieRequest request, CancellationToken ct)
    {
        var movie = new Movie
        {
            Title = request.Title.Trim(), Synopsis = request.Synopsis.Trim(), Director = request.Director.Trim(),
            Genre = request.Genre.Trim(), ReleaseYear = request.ReleaseYear,
            Actors = request.Actors.Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(x => new MovieActor { Name = x }).ToList()
        };
        db.Movies.Add(movie);
        await db.SaveChangesAsync(ct);
        return (await GetAsync(movie.Id, ct))!;
    }

    public Task<ListResponse<MovieSummary>> ListAsync(MovieQuery request, CancellationToken ct)
    {
        var query = db.Movies.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Name)) query = query.Where(x => x.Title.Contains(request.Name.Trim()));
        if (!string.IsNullOrWhiteSpace(request.Director)) query = query.Where(x => x.Director.Contains(request.Director.Trim()));
        if (!string.IsNullOrWhiteSpace(request.Genre)) query = query.Where(x => x.Genre.Contains(request.Genre.Trim()));
        if (!string.IsNullOrWhiteSpace(request.Actor)) query = query.Where(x => x.Actors.Any(a => a.Name.Contains(request.Actor.Trim())));
        // Order mapped entities before projecting records, allowing translation to SQL.
        var ordered = request.Sort == MovieSort.Title
            ? query.OrderBy(x => x.Title).ThenBy(x => x.Id)
            : query.OrderByDescending(x => x.Votes.Count).ThenBy(x => x.Title).ThenBy(x => x.Id);
        return ordered.Select(x => new MovieSummary(x.Id, x.Title, x.Director, x.Genre, x.ReleaseYear,
            x.Votes.Count, x.Votes.Select(v => (double?)v.Score).Average() ?? 0)).ToListResponseAsync(request, ct);
    }

    public Task<MovieDetails?> GetAsync(Guid id, CancellationToken ct) => db.Movies.AsNoTracking()
        .Where(x => x.Id == id)
        .Select(x => new MovieDetails(x.Id, x.Title, x.Synopsis, x.Director, x.Genre, x.ReleaseYear,
            x.Actors.OrderBy(a => a.Name).Select(a => a.Name).ToList(), x.Votes.Count,
            x.Votes.Select(v => (double?)v.Score).Average() ?? 0))
        .SingleOrDefaultAsync(ct);

    public async Task VoteAsync(Guid movieId, Guid userId, int score, CancellationToken ct)
    {
        var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == userId, ct)
            ?? throw new ApiException(401, "Conta inválida.");
        if (user.IsAdministrator) throw new ApiException(403, "Administradores não podem votar.");
        if (!await db.Movies.AnyAsync(x => x.Id == movieId, ct)) throw new ApiException(404, "Filme não encontrado.");
        var vote = await db.Votes.FindAsync([movieId, userId], ct);
        if (vote is null) db.Votes.Add(new Vote(movieId, userId, score));
        else vote.ChangeScore(score);
        await db.SaveChangesAsync(ct);
    }
}
