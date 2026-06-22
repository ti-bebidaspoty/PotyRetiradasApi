using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PotyRetiradasApi.Dtos.Colaboradores;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Controllers;

[Authorize]
[ApiController]
[Route("api/colaboradores")]
public sealed class ColaboradoresController : ControllerBase
{
    private readonly IColaboradorService _service;

    public ColaboradoresController(IColaboradorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ColaboradorResponse>>> Listar(
        CancellationToken cancellationToken)
    {
        var colaboradores = await _service.ListarAsync(
            cancellationToken);

        return Ok(colaboradores);
    }

    [HttpGet("{colaboradorId}", Name = "ObterColaboradorPorId")]
    public async Task<ActionResult<ColaboradorResponse>> ObterPorId(
        string colaboradorId,
        CancellationToken cancellationToken)
    {
        var colaborador = await _service.ObterPorIdAsync(
            colaboradorId,
            cancellationToken);

        if (colaborador is null)
        {
            return NotFound(new
            {
                mensagem = $"Colaborador {colaboradorId} não encontrado."
            });
        }

        return Ok(colaborador);
    }

    [HttpPost]
    public async Task<ActionResult<ColaboradorResponse>> Criar(
    CriarColaboradorRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var colaborador = await _service.CriarAsync(
                request,
                cancellationToken);

            return CreatedAtRoute(
                "ObterColaboradorPorId",
                new { colaboradorId = colaborador.ColaboradorId },
                colaborador);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpPut("{colaboradorId}")]
    public async Task<ActionResult<ColaboradorResponse>> Atualizar(
        string colaboradorId,
        AtualizarColaboradorRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var colaborador = await _service.AtualizarAsync(
                colaboradorId,
                request,
                cancellationToken);

            if (colaborador is null)
            {
                return NotFound(new
                {
                    mensagem = $"Colaborador {colaboradorId} não encontrado."
                });
            }

            return Ok(colaborador);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpDelete("{colaboradorId}")]
    public async Task<IActionResult> Desativar(
        string colaboradorId,
        CancellationToken cancellationToken)
    {
        var desativado = await _service.DesativarAsync(
            colaboradorId,
            cancellationToken);

        if (!desativado)
        {
            return NotFound(new
            {
                mensagem = $"Colaborador {colaboradorId} não encontrado."
            });
        }

        return NoContent();
    }
}