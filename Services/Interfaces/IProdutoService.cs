using PotyRetiradasApi.Dtos.Produtos;

namespace PotyRetiradasApi.Services.Interfaces;

public interface IProdutoService
{
    Task<IReadOnlyList<ProdutoResponse>> ListarAsync(
        CancellationToken cancellationToken);

    Task<ProdutoResponse?> ObterPorIdAsync(
        string produtoId,
        CancellationToken cancellationToken);

    Task<ProdutoResponse?> CriarAsync(
        CriarProdutoRequest request,
        CancellationToken cancellationToken);

    Task<ProdutoResponse?> AtualizarAsync(
        string produtoId,
        AtualizarProdutoRequest request,
        CancellationToken cancellationToken);

    Task<bool> DesativarAsync(
        string produtoId,
        CancellationToken cancellationToken);
}
