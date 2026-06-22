using PotyRetiradasApi.Dtos.Unidades;

namespace PotyRetiradasApi.Services.Interfaces;

public interface IUnidadeService
{
    Task<IReadOnlyList<UnidadeResponse>> ListarAsync(
        CancellationToken cancellationToken);

    Task<UnidadeResponse?> ObterPorIdAsync(
        int unidadeId,
        CancellationToken cancellationToken);

    Task<UnidadeResponse?> CriarAsync(
        CriarUnidadeRequest request,
        CancellationToken cancellationToken);

    Task<UnidadeResponse?> AtualizarAsync(
        int unidadeId,
        AtualizarUnidadeRequest request,
        CancellationToken cancellationToken);

    Task<bool> ExcluirAsync(
        int unidadeId,
        CancellationToken cancellationToken);
}