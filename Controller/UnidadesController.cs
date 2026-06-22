using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PotyRetiradasApi.Dtos.Unidades;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Controllers;

[Authorize]
[ApiController]
[Route("api/unidades")]
public sealed class UnidadesController : ControllerBase
{
    private readonly IUnidadeService _service;

    public UnidadesController(IUnidadeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UnidadeResponse>>> Listar(
        CancellationToken cancellationToken)
    {
        var unidades = await _service.ListarAsync(
            cancellationToken);

        return Ok(unidades);
    }

    [HttpGet("{unidadeId:int}", Name = "ObterUnidadePorId")]
    public async Task<ActionResult<UnidadeResponse>> ObterPorId(
        int unidadeId,
        CancellationToken cancellationToken)
    {
        var unidade = await _service.ObterPorIdAsync(
            unidadeId,
            cancellationToken);

        if (unidade is null)
        {
            return NotFound(new
            {
                mensagem = $"Unidade {unidadeId} não encontrada."
            });
        }

        return Ok(unidade);
    }

    [HttpPost]
    public async Task<ActionResult<UnidadeResponse>> Criar(
        CriarUnidadeRequest request,
        CancellationToken cancellationToken)
    {
        var unidade = await _service.CriarAsync(
            request,
            cancellationToken);

        if (unidade is null)
        {
            return Conflict(new
            {
                mensagem =
                    $"Já existe uma unidade com o ID {request.UnidadeId}."
            });
        }

        return CreatedAtRoute(
            "ObterUnidadePorId",
            new { unidadeId = unidade.UnidadeId },
            unidade);
    }

    [HttpPut("{unidadeId:int}")]
    public async Task<ActionResult<UnidadeResponse>> Atualizar(
        int unidadeId,
        AtualizarUnidadeRequest request,
        CancellationToken cancellationToken)
    {
        var unidade = await _service.AtualizarAsync(
            unidadeId,
            request,
            cancellationToken);

        if (unidade is null)
        {
            return NotFound(new
            {
                mensagem = $"Unidade {unidadeId} não encontrada."
            });
        }

        return Ok(unidade);
    }

    [HttpDelete("{unidadeId:int}")]
    public async Task<IActionResult> Excluir(
        int unidadeId,
        CancellationToken cancellationToken)
    {
        var excluida = await _service.ExcluirAsync(
            unidadeId,
            cancellationToken);

        if (!excluida)
        {
            return NotFound(new
            {
                mensagem = $"Unidade {unidadeId} não encontrada."
            });
        }

        return NoContent();
    }
}