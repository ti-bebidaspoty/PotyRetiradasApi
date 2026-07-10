using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PotyRetiradasApi.Dtos.RetiradasMensais;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Controllers;

[Authorize]
[ApiController]
[Route("api/retiradas-mensais-produtos")]
public sealed class RetiradasMensaisProdutosController : ControllerBase
{
    private readonly IRetiradaMensalProdutoService _service;

    public RetiradasMensaisProdutosController(
        IRetiradaMensalProdutoService service)
    {
        _service = service;
    }

    [HttpPost("{retiradaMensalId}")]
    public async Task<ActionResult<ProdutoRetiradaResponse>> Adicionar(
        string retiradaMensalId,
        ProdutoRetiradaRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var produto = await _service.AdicionarAsync(
                retiradaMensalId,
                request,
                cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                produto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpDelete("{retiradaMensalProdutoId}")]
    public async Task<IActionResult> Excluir(
        string retiradaMensalProdutoId,
        CancellationToken cancellationToken)
    {
        try
        {
            var excluido = await _service.ExcluirAsync(
                retiradaMensalProdutoId,
                cancellationToken);

            if (!excluido)
            {
                return NotFound(new
                {
                    mensagem =
                        $"Produto da retirada {retiradaMensalProdutoId} não encontrado."
                });
            }

            return NoContent();
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