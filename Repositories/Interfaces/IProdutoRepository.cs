using Microsoft.EntityFrameworkCore;
using PotyRetiradasApi.Data;
using PotyRetiradasApi.Entities;

namespace PotyRetiradasApi.Repositories.Interfaces;

public interface IProdutoRepository
{
    Task<IReadOnlyList<Produto>> ListarAsync(
        CancellationToken cancellationToken);

    Task<Produto?> ObterPorIdAsync(
        string produtoId,
        bool rastrear,
        CancellationToken cancellationToken);

    Task<bool> ExisteAsync(
        string produtoId,
        CancellationToken cancellationToken);

    Task AdicionarAsync(
        Produto produto,
        CancellationToken cancellationToken);

    Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken);
}
