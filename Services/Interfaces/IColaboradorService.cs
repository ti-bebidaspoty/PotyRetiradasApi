using PotyRetiradasApi.Dtos.Colaboradores;

namespace PotyRetiradasApi.Services.Interfaces;

public interface IColaboradorService
{
    Task<IReadOnlyList<ColaboradorResponse>> ListarAsync(
        CancellationToken cancellationToken);

    Task<ColaboradorResponse?> ObterPorIdAsync(
        string colaboradorId,
        CancellationToken cancellationToken);

    Task<ColaboradorResponse> CriarAsync(
        CriarColaboradorRequest request,
        CancellationToken cancellationToken);

    Task<ColaboradorResponse?> AtualizarAsync(
        string colaboradorId,
        AtualizarColaboradorRequest request,
        CancellationToken cancellationToken);

    Task<bool> DesativarAsync(
        string colaboradorId,
        CancellationToken cancellationToken);
}