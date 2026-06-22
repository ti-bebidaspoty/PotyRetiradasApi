using PotyRetiradasApi.Dtos.Tipos;

namespace PotyRetiradasApi.Services.Interfaces;

public interface ITipoService
{
    Task<IReadOnlyList<TipoResponse>> ListarAsync(
        CancellationToken cancellationToken);

    Task<TipoResponse?> ObterPorIdAsync(
        string tipoId,
        CancellationToken cancellationToken);

    Task<TipoResponse> CriarAsync(
        CriarTipoRequest request,
        CancellationToken cancellationToken);

    Task<TipoResponse?> AtualizarAsync(
        string tipoId,
        AtualizarTipoRequest request,
        CancellationToken cancellationToken);

    Task<bool> ExcluirAsync(
        string tipoId,
        CancellationToken cancellationToken);
}