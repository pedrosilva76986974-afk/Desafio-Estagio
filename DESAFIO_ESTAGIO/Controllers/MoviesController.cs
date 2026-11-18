using System.Security.Claims;
using DESAFIO_ESTAGIO.Contracts;
using DESAFIO_ESTAGIO.Security;
using DESAFIO_ESTAGIO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DESAFIO_ESTAGIO.Controllers;

[ApiController, Route("api/movies")]
public sealed class MoviesController(MovieService movies) : ControllerBase
{
    [HttpPost, Authorize(Roles = Roles.Administrator)]
    [ProducesResponseType<MovieDetails>(StatusCodes.Status201Created)]
    public async Task<ActionResult<MovieDetails>> Create(CreateMovieRequest request, CancellationToken ct)
    {
        var movie = await movies.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie);
    }

    [HttpGet, AllowAnonymous]
    public async Task<ActionResult<ListResponse<MovieSummary>>> List([FromQuery] MovieQuery query, CancellationToken ct) =>
        Ok(await movies.ListAsync(query, ct));

    [HttpGet("{id:guid}"), AllowAnonymous]
    [ProducesResponseType<MovieDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDetails>> Get(Guid id, CancellationToken ct) =>
        await movies.GetAsync(id, ct) is { } movie ? Ok(movie) : NotFound();

    [HttpPut("{id:guid}/vote"), Authorize(Roles = Roles.User)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Vote(Guid id, VoteRequest request, CancellationToken ct)
    {
        await movies.VoteAsync(id, Guid.Parse(User.FindFirstValue("sub")!), request.Score!.Value, ct);
        return NoContent();
    }
}
