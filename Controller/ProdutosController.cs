using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PotyRetiradasApi.Dtos.Produtos;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Controllers;

//[Authorize]
[ApiController]
[Route("api/produtos")]
public sealed class ProdutosController : ControllerBase
{
    private readonly IProdutoService _service;

    public ProdutosController(IProdutoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProdutoResponse>>> Listar(
        CancellationToken cancellationToken)
    {
        var produtos = await _service.ListarAsync(
            cancellationToken);

        return Ok(produtos);
    }

    [HttpGet("{produtoId}", Name = "ObterProdutoPorId")]
    public async Task<ActionResult<ProdutoResponse>> ObterPorId(
        string produtoId,
        CancellationToken cancellationToken)
    {
        var produto = await _service.ObterPorIdAsync(
            produtoId,
            cancellationToken);

        if (produto is null)
        {
            return NotFound(new
            {
                mensagem = $"Produto {produtoId} não encontrado."
            });
        }

        return Ok(produto);
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoResponse>> Criar(
        CriarProdutoRequest request,
        CancellationToken cancellationToken)
    {
        var produto = await _service.CriarAsync(
            request,
            cancellationToken);

        if (produto is null)
        {
            return Conflict(new
            {
                mensagem =
                    $"Já existe um produto com o código {request.ProdutoId}."
            });
        }

        return CreatedAtRoute(
            "ObterProdutoPorId",
            new { produtoId = produto.ProdutoId },
            produto);
    }

    [HttpPut("{produtoId}")]
    public async Task<ActionResult<ProdutoResponse>> Atualizar(
        string produtoId,
        AtualizarProdutoRequest request,
        CancellationToken cancellationToken)
    {
        var produto = await _service.AtualizarAsync(
            produtoId,
            request,
            cancellationToken);

        if (produto is null)
        {
            return NotFound(new
            {
                mensagem = $"Produto {produtoId} não encontrado."
            });
        }

        return Ok(produto);
    }

    [HttpDelete("{produtoId}")]
    public async Task<IActionResult> Desativar(
        string produtoId,
        CancellationToken cancellationToken)
    {
        var desativado = await _service.DesativarAsync(
            produtoId,
            cancellationToken);

        if (!desativado)
        {
            return NotFound(new
            {
                mensagem = $"Produto {produtoId} não encontrado."
            });
        }

        return NoContent();
    }
}
