using DESAFIO_ESTAGIO.Contracts;
using DESAFIO_ESTAGIO.Security;
using DESAFIO_ESTAGIO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DESAFIO_ESTAGIO.Controllers;

[ApiController, Route("api/administrators"), Authorize(Roles = Roles.Administrator)]
public sealed class AdministratorsController(UserService users) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserResponse>> Create(CreateUserRequest request, CancellationToken ct)
    {
        var user = await users.CreateAsync(request, true, ct);
        return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    }

    [HttpGet]
    public async Task<ActionResult<ListResponse<UserResponse>>> List([FromQuery] ListQuery query, CancellationToken ct) =>
        Ok(await users.ListAsync(true, query, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> Get(Guid id, CancellationToken ct) => Ok(await users.GetAsync(id, true, ct));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponse>> Update(Guid id, UpdateUserRequest request, CancellationToken ct) =>
        Ok(await users.UpdateAsync(id, true, request, ct));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await users.DeleteAsync(id, true, ct);
        return NoContent();
    }
}
