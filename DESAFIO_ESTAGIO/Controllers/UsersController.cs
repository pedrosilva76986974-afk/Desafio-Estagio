using System.Security.Claims;
using DESAFIO_ESTAGIO.Contracts;
using DESAFIO_ESTAGIO.Security;
using DESAFIO_ESTAGIO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DESAFIO_ESTAGIO.Controllers;

[ApiController, Route("api/users"), Authorize]
public sealed class UsersController(UserService users) : ControllerBase
{
    [HttpPost, AllowAnonymous, EnableRateLimiting("auth")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserResponse>> Register(CreateUserRequest request, CancellationToken ct)
    {
        var user = await users.CreateAsync(request, false, ct);
        return Created("/api/users/me", user);
    }

    [HttpGet, Authorize(Roles = Roles.Administrator)]
    public async Task<ActionResult<ListResponse<UserResponse>>> List([FromQuery] ListQuery query, CancellationToken ct) =>
        Ok(await users.ListAsync(false, query, ct));

    [HttpGet("me"), Authorize(Roles = Roles.User)]
    public async Task<ActionResult<UserResponse>> GetMe(CancellationToken ct) => Ok(await users.GetAsync(CurrentId, false, ct));

    [HttpPut("me"), Authorize(Roles = Roles.User)]
    public async Task<ActionResult<UserResponse>> UpdateMe(UpdateUserRequest request, CancellationToken ct) =>
        Ok(await users.UpdateAsync(CurrentId, false, request, ct));

    [HttpDelete("me"), Authorize(Roles = Roles.User)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMe(CancellationToken ct)
    {
        await users.DeleteAsync(CurrentId, false, ct);
        return NoContent();
    }

    [HttpGet("{id:guid}"), Authorize(Roles = Roles.Administrator)]
    public async Task<ActionResult<UserResponse>> Get(Guid id, CancellationToken ct) => Ok(await users.GetAsync(id, false, ct));

    [HttpPut("{id:guid}"), Authorize(Roles = Roles.Administrator)]
    public async Task<ActionResult<UserResponse>> Update(Guid id, UpdateUserRequest request, CancellationToken ct) =>
        Ok(await users.UpdateAsync(id, false, request, ct));

    [HttpDelete("{id:guid}"), Authorize(Roles = Roles.Administrator)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await users.DeleteAsync(id, false, ct);
        return NoContent();
    }

    private Guid CurrentId => Guid.Parse(User.FindFirstValue("sub")!);
}
