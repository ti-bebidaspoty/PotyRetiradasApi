using PotyRetiradasApi.Entities;

namespace PotyRetiradasApi.Repositories.Interfaces;

public interface IColaboradorRepository
{
    Task<IReadOnlyList<Colaboradore>> ListarAsync(
        CancellationToken cancellationToken);

    Task<Colaboradore?> ObterPorIdAsync(
        string colaboradorId,
        bool rastrear,
        CancellationToken cancellationToken);

    Task<bool> ExisteAsync(
        string colaboradorId,
        CancellationToken cancellationToken);

    Task AdicionarAsync(
        Colaboradore colaborador,
        CancellationToken cancellationToken);

    Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken);
}