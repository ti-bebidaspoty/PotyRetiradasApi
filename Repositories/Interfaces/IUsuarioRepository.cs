using PotyRetiradasApi.Entities;

namespace PotyRetiradasApi.Repositories.Interfaces;

public interface IUsuarioRepository
{
    Task<IReadOnlyList<Usuario>> ListarAsync(
        CancellationToken cancellationToken);

    Task<Usuario?> ObterPorIdAsync(
        string usuarioId,
        bool rastrear,
        CancellationToken cancellationToken);

    Task<bool> ExisteLoginAsync(
        string login,
        string? ignorarUsuarioId,
        CancellationToken cancellationToken);

    Task AdicionarAsync(
        Usuario usuario,
        CancellationToken cancellationToken);

    Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken);

    Task<Usuario?> ObterPorLoginAsync(
    string login,
    CancellationToken cancellationToken);
}