using PotyRetiradasApi.Entities;

namespace PotyRetiradasApi.Repositories.Interfaces;

public interface IRetiradaMensalProdutoRepository
{
    Task<IReadOnlyList<RetiradasMensaisProduto>> ListarPorRetiradaMensalIdAsync(
        string retiradaMensalId,
        CancellationToken cancellationToken);

    Task AdicionarVariosAsync(
        IEnumerable<RetiradasMensaisProduto> produtos,
        CancellationToken cancellationToken);

    void RemoverVarios(
        IEnumerable<RetiradasMensaisProduto> produtos);

    Task<bool> ExisteProdutoNaRetiradaAsync(
        string retiradaMensalId,
        string produtoId,
        CancellationToken cancellationToken);

    Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken);
}