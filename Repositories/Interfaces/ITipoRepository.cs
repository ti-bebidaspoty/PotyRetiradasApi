using PotyRetiradasApi.Entities;

namespace PotyRetiradasApi.Repositories.Interfaces;

public interface ITipoRepository
{
    Task<IReadOnlyList<Tipo>> ListarAsync(
        CancellationToken cancellationToken);

    Task<Tipo?> ObterPorIdAsync(
        string tipoId,
        bool rastrear,
        CancellationToken cancellationToken);

    Task<bool> ExisteAsync(
        string tipoId,
        CancellationToken cancellationToken);

    Task AdicionarAsync(
        Tipo tipo,
        CancellationToken cancellationToken);

    void Remover(Tipo tipo);

    Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken);
}