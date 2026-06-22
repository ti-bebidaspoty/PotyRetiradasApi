using Microsoft.AspNetCore.Mvc;
using PotyRetiradasApi.Dtos.Auth;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _service.LoginAsync(
            request,
            cancellationToken);

        if (response is null)
        {
            return Unauthorized(new
            {
                mensagem = "Usuário ou senha inválidos."
            });
        }

        return Ok(response);
    }
}