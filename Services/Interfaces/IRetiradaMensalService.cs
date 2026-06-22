using PotyRetiradasApi.Dtos.RetiradasMensais;

namespace PotyRetiradasApi.Services.Interfaces;

public interface IRetiradaMensalService
{
    Task<IReadOnlyList<RetiradaMensalResponse>> ListarAsync(
        CancellationToken cancellationToken);

    Task<RetiradaMensalResponse?> ObterPorIdAsync(
        string retiradaMensalId,
        CancellationToken cancellationToken);

    Task<RetiradaMensalResponse> CriarAsync(
        CriarRetiradaMensalRequest request,
        CancellationToken cancellationToken);

    Task<bool> ExcluirAsync(
        string retiradaMensalId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<RetiradaMensalResponse>> ListarPorAnoMesAsync(
    string anoMes,
    CancellationToken cancellationToken);
}