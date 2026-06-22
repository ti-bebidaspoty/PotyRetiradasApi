using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PotyRetiradasApi.Dtos.RetiradasMensais;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Controllers;

[Authorize]
[ApiController]
[Route("api/retiradas-mensais")]
public sealed class RetiradasMensaisController : ControllerBase
{
    private readonly IRetiradaMensalService _service;

    public RetiradasMensaisController(
        IRetiradaMensalService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RetiradaMensalResponse>>> Listar(
        CancellationToken cancellationToken)
    {
        var retiradas = await _service.ListarAsync(
            cancellationToken);

        return Ok(retiradas);
    }

    [HttpGet("{retiradaMensalId}", Name = "ObterRetiradaMensalPorId")]
    public async Task<ActionResult<RetiradaMensalResponse>> ObterPorId(
        string retiradaMensalId,
        CancellationToken cancellationToken)
    {
        var retirada = await _service.ObterPorIdAsync(
            retiradaMensalId,
            cancellationToken);

        if (retirada is null)
        {
            return NotFound(new
            {
                mensagem = $"Retirada {retiradaMensalId} não encontrada."
            });
        }

        return Ok(retirada);
    }

    [HttpPost]
    public async Task<ActionResult<RetiradaMensalResponse>> Criar(
        CriarRetiradaMensalRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var retirada = await _service.CriarAsync(
                request,
                cancellationToken);

            return CreatedAtRoute(
                "ObterRetiradaMensalPorId",
                new { retiradaMensalId = retirada.RetiradaMensalId },
                retirada);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpDelete("{retiradaMensalId}")]
    public async Task<IActionResult> Excluir(
        string retiradaMensalId,
        CancellationToken cancellationToken)
    {
        var excluido = await _service.ExcluirAsync(
            retiradaMensalId,
            cancellationToken);

        if (!excluido)
        {
            return NotFound(new
            {
                mensagem = $"Retirada {retiradaMensalId} não encontrada."
            });
        }

        return NoContent();
    }

    [HttpGet("ano-mes/{anoMes}")]
    public async Task<ActionResult<IReadOnlyList<RetiradaMensalResponse>>> ListarPorAnoMes(
    string anoMes,
    CancellationToken cancellationToken)
    {
        try
        {
            var retiradas = await _service.ListarPorAnoMesAsync(
                anoMes,
                cancellationToken);

            return Ok(retiradas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }
}