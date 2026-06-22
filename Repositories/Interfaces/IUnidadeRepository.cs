using PotyRetiradasApi.Entities;

namespace PotyRetiradasApi.Repositories.Interfaces;

public interface IUnidadeRepository
{
    Task<IReadOnlyList<Unidade>> ListarAsync(
        CancellationToken cancellationToken);

    Task<Unidade?> ObterPorIdAsync(
        int unidadeId,
        bool rastrear,
        CancellationToken cancellationToken);

    Task<bool> ExisteAsync(
        int unidadeId,
        CancellationToken cancellationToken);

    Task AdicionarAsync(
        Unidade unidade,
        CancellationToken cancellationToken);

    void Remover(Unidade unidade);

    Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken);
}