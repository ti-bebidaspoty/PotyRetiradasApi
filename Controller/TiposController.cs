using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PotyRetiradasApi.Dtos.Tipos;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Controllers;

[Authorize]
[ApiController]
[Route("api/tipos")]
public sealed class TiposController : ControllerBase
{
    private readonly ITipoService _service;

    public TiposController(ITipoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TipoResponse>>> Listar(
        CancellationToken cancellationToken)
    {
        var tipos = await _service.ListarAsync(cancellationToken);

        return Ok(tipos);
    }

    [HttpGet("{tipoId}", Name = "ObterTipoPorId")]
    public async Task<ActionResult<TipoResponse>> ObterPorId(
        string tipoId,
        CancellationToken cancellationToken)
    {
        var tipo = await _service.ObterPorIdAsync(
            tipoId,
            cancellationToken);

        if (tipo is null)
        {
            return NotFound(new
            {
                mensagem = $"Tipo {tipoId} não encontrado."
            });
        }

        return Ok(tipo);
    }

    [HttpPost]
    public async Task<ActionResult<TipoResponse>> Criar(
    CriarTipoRequest request,
    CancellationToken cancellationToken)
    {
        var tipo = await _service.CriarAsync(
            request,
            cancellationToken);

        return CreatedAtRoute(
            "ObterTipoPorId",
            new { tipoId = tipo.TipoId },
            tipo);
    }

    [HttpPut("{tipoId}")]
    public async Task<ActionResult<TipoResponse>> Atualizar(
        string tipoId,
        AtualizarTipoRequest request,
        CancellationToken cancellationToken)
    {
        var tipo = await _service.AtualizarAsync(
            tipoId,
            request,
            cancellationToken);

        if (tipo is null)
        {
            return NotFound(new
            {
                mensagem = $"Tipo {tipoId} não encontrado."
            });
        }

        return Ok(tipo);
    }

    [HttpDelete("{tipoId}")]
    public async Task<IActionResult> Excluir(
    string tipoId,
    CancellationToken cancellationToken)
    {
        try
        {
            var excluido = await _service.ExcluirAsync(
                tipoId,
                cancellationToken);

            if (!excluido)
            {
                return NotFound(new
                {
                    mensagem = $"Tipo {tipoId} não encontrado."
                });
            }

            return NoContent();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            return Conflict(new
            {
                mensagem = "Não foi possível excluir o tipo porque ele está vinculado a um ou mais colaboradores."
            });
        }
    }
}