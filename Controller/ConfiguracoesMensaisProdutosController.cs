using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PotyRetiradasApi.Dtos.ConfiguracoesMensaisProdutos;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Controllers;

[Authorize]
[ApiController]
[Route("api/configuracoes-mensais-produtos")]
public sealed class ConfiguracoesMensaisProdutosController : ControllerBase
{
    private readonly IConfiguracaoMensalProdutoService _service;

    public ConfiguracoesMensaisProdutosController(
        IConfiguracaoMensalProdutoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ConfiguracaoMensalProdutoResponse>>> Listar(
        CancellationToken cancellationToken)
    {
        var configuracoes = await _service.ListarAsync(
            cancellationToken);

        return Ok(configuracoes);
    }

    [HttpGet("{configuracaoMensalId}", Name = "ObterConfiguracaoMensalProdutoPorId")]
    public async Task<ActionResult<ConfiguracaoMensalProdutoResponse>> ObterPorId(
        string configuracaoMensalId,
        CancellationToken cancellationToken)
    {
        var configuracao = await _service.ObterPorIdAsync(
            configuracaoMensalId,
            cancellationToken);

        if (configuracao is null)
        {
            return NotFound(new
            {
                mensagem = $"Configuração {configuracaoMensalId} não encontrada."
            });
        }

        return Ok(configuracao);
    }

    [HttpPost]
    public async Task<ActionResult<ConfiguracaoMensalProdutoResponse>> Criar(
        CriarConfiguracaoMensalProdutoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var configuracao = await _service.CriarAsync(
                request,
                cancellationToken);

            if (configuracao is null)
            {
                return Conflict(new
                {
                    mensagem = "Já existe uma configuração para este mês, unidade e produto."
                });
            }

            return CreatedAtRoute(
                "ObterConfiguracaoMensalProdutoPorId",
                new { configuracaoMensalId = configuracao.ConfiguracaoMensalId },
                configuracao);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpPut("{configuracaoMensalId}")]
    public async Task<ActionResult<ConfiguracaoMensalProdutoResponse>> Atualizar(
        string configuracaoMensalId,
        AtualizarConfiguracaoMensalProdutoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var configuracao = await _service.AtualizarAsync(
                configuracaoMensalId,
                request,
                cancellationToken);

            if (configuracao is null)
            {
                return NotFound(new
                {
                    mensagem = $"Configuração {configuracaoMensalId} não encontrada."
                });
            }

            return Ok(configuracao);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }

    [HttpDelete("{configuracaoMensalId}")]
    public async Task<IActionResult> Excluir(
        string configuracaoMensalId,
        CancellationToken cancellationToken)
    {
        var excluido = await _service.ExcluirAsync(
            configuracaoMensalId,
            cancellationToken);

        if (!excluido)
        {
            return NotFound(new
            {
                mensagem = $"Configuração {configuracaoMensalId} não encontrada."
            });
        }

        return NoContent();
    }

    [HttpGet("ano-mes/{anoMes}/unidade/{unidadeId:int}/produtos")]
    public async Task<ActionResult<IReadOnlyList<ProdutoConfiguradoMesResponse>>> ListarProdutosConfigurados(
    string anoMes,
    int unidadeId,
    CancellationToken cancellationToken)
    {
        try
        {
            var produtos = await _service.ListarProdutosConfiguradosAsync(
                anoMes,
                unidadeId,
                cancellationToken);

            return Ok(produtos);
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