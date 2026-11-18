using DESAFIO_ESTAGIO.Contracts;
using DESAFIO_ESTAGIO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DESAFIO_ESTAGIO.Controllers;

[ApiController, Route("api/auth"), AllowAnonymous, EnableRateLimiting("auth")]
public sealed class AuthController(UserService users) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> Login(LoginRequest request) => Ok(await users.LoginAsync(request));
}
