using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PotyRetiradasApi.Dtos.Usuarios;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Controllers;

[Authorize]
[ApiController]
[Route("api/usuarios")]
public sealed class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UsuarioResponse>>> Listar(
        CancellationToken cancellationToken)
    {
        var usuarios = await _service.ListarAsync(
            cancellationToken);

        return Ok(usuarios);
    }

    [HttpGet("{usuarioId}", Name = "ObterUsuarioPorId")]
    public async Task<ActionResult<UsuarioResponse>> ObterPorId(
        string usuarioId,
        CancellationToken cancellationToken)
    {
        var usuario = await _service.ObterPorIdAsync(
            usuarioId,
            cancellationToken);

        if (usuario is null)
        {
            return NotFound(new
            {
                mensagem = $"Usuário {usuarioId} não encontrado."
            });
        }

        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioResponse>> Criar(
        CriarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuario = await _service.CriarAsync(
                request,
                cancellationToken);

            if (usuario is null)
            {
                return Conflict(new
                {
                    mensagem = $"Já existe um usuário com o login {request.Usuario}."
                });
            }

            return CreatedAtRoute(
                "ObterUsuarioPorId",
                new { usuarioId = usuario.UsuarioId },
                usuario);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpPut("{usuarioId}")]
    public async Task<ActionResult<UsuarioResponse>> Atualizar(
        string usuarioId,
        AtualizarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuario = await _service.AtualizarAsync(
                usuarioId,
                request,
                cancellationToken);

            if (usuario is null)
            {
                return NotFound(new
                {
                    mensagem = $"Usuário {usuarioId} não encontrado."
                });
            }

            return Ok(usuario);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpDelete("{usuarioId}")]
    public async Task<IActionResult> Desativar(
        string usuarioId,
        CancellationToken cancellationToken)
    {
        var desativado = await _service.DesativarAsync(
            usuarioId,
            cancellationToken);

        if (!desativado)
        {
            return NotFound(new
            {
                mensagem = $"Usuário {usuarioId} não encontrado."
            });
        }

        return NoContent();
    }
}