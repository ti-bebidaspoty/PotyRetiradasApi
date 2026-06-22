using PotyRetiradasApi.Entities;

namespace PotyRetiradasApi.Repositories.Interfaces;

public interface IRetiradaMensalRepository
{
    Task<IReadOnlyList<RetiradasMensai>> ListarAsync(
        CancellationToken cancellationToken);

    Task<RetiradasMensai?> ObterPorIdAsync(
        string retiradaMensalId,
        bool rastrear,
        CancellationToken cancellationToken);

    Task AdicionarAsync(
        RetiradasMensai retirada,
        CancellationToken cancellationToken);

    void Remover(
        RetiradasMensai retirada);

    Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken);

    Task<bool> ExisteRetiradaDoColaboradorNoMesAsync(
    string anoMes,
    string colaboradorId,
    CancellationToken cancellationToken);

    Task<IReadOnlyList<RetiradasMensai>> ListarPorAnoMesAsync(
    string anoMes,
    CancellationToken cancellationToken);
}