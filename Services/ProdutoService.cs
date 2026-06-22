using PotyRetiradasApi.Dtos.Produtos;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Services;

public sealed class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repository;
    private readonly IArquivoStorageService _arquivoStorageService;

    public ProdutoService(
        IProdutoRepository repository,
        IArquivoStorageService arquivoStorageService)
    {
        _repository = repository;
        _arquivoStorageService = arquivoStorageService;
    }

    public async Task<IReadOnlyList<ProdutoResponse>> ListarAsync(
        CancellationToken cancellationToken)
    {
        var produtos = await _repository.ListarAsync(
            cancellationToken);

        return produtos
            .Select(MapearResponse)
            .ToList();
    }

    public async Task<ProdutoResponse?> ObterPorIdAsync(
        string produtoId,
        CancellationToken cancellationToken)
    {
        produtoId = NormalizarProdutoId(produtoId);

        var produto = await _repository.ObterPorIdAsync(
            produtoId,
            rastrear: false,
            cancellationToken);

        return produto is null
            ? null
            : MapearResponse(produto);
    }

    public async Task<ProdutoResponse?> CriarAsync(
    CriarProdutoRequest request,
    CancellationToken cancellationToken)
    {
        var produtoId = NormalizarProdutoId(request.ProdutoId);

        var produtoJaExiste = await _repository.ExisteAsync(
            produtoId,
            cancellationToken);

        if (produtoJaExiste)
        {
            return null;
        }

        string? urlImagem = null;

        if (!string.IsNullOrWhiteSpace(request.Imagem))
        {
            urlImagem = await _arquivoStorageService.UploadImagemBase64Async(
                request.Imagem,
                produtoId,
                cancellationToken);
        }

        var produto = new Produto
        {
            ProdutoId = produtoId,
            Descricao = request.Descricao.Trim(),
            CodigoBarras = NormalizarTextoOpcional(request.CodigoBarras),
            Imagem = urlImagem,
            Status = request.Status
        };

        await _repository.AdicionarAsync(
            produto,
            cancellationToken);

        await _repository.SalvarAlteracoesAsync(
            cancellationToken);

        return MapearResponse(produto);
    }

    public async Task<ProdutoResponse?> AtualizarAsync(
    string produtoId,
    AtualizarProdutoRequest request,
    CancellationToken cancellationToken)
    {
        produtoId = NormalizarProdutoId(produtoId);

        var produto = await _repository.ObterPorIdAsync(
            produtoId,
            rastrear: true,
            cancellationToken);

        if (produto is null)
        {
            return null;
        }

        produto.Descricao = request.Descricao.Trim();
        produto.CodigoBarras = NormalizarTextoOpcional(request.CodigoBarras);
        produto.Status = request.Status;

        if (!string.IsNullOrWhiteSpace(request.Imagem))
        {
            var urlImagem = await _arquivoStorageService.UploadImagemBase64Async(
                request.Imagem,
                produtoId,
                cancellationToken);

            produto.Imagem = urlImagem;
        }

        await _repository.SalvarAlteracoesAsync(
            cancellationToken);

        return MapearResponse(produto);
    }

    public async Task<bool> DesativarAsync(
        string produtoId,
        CancellationToken cancellationToken)
    {
        produtoId = NormalizarProdutoId(produtoId);

        var produto = await _repository.ObterPorIdAsync(
            produtoId,
            rastrear: true,
            cancellationToken);

        if (produto is null)
        {
            return false;
        }

        produto.Status = false;

        await _repository.SalvarAlteracoesAsync(
            cancellationToken);

        return true;
    }

    private static ProdutoResponse MapearResponse(
        Produto produto)
    {
        return new ProdutoResponse
        {
            ProdutoId = produto.ProdutoId,
            Descricao = produto.Descricao,
            Status = produto.Status,
            CodigoBarras = produto.CodigoBarras,
            Imagem = produto.Imagem
        };
    }

    private static string NormalizarProdutoId(
        string produtoId)
    {
        return produtoId.Trim().ToUpperInvariant();
    }

    private static string? NormalizarTextoOpcional(
        string? texto)
    {
        return string.IsNullOrWhiteSpace(texto)
            ? null
            : texto.Trim();
    }
}
