using PotyRetiradasApi.Dtos.Usuarios;

namespace PotyRetiradasApi.Services.Interfaces;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioResponse>> ListarAsync(
        CancellationToken cancellationToken);

    Task<UsuarioResponse?> ObterPorIdAsync(
        string usuarioId,
        CancellationToken cancellationToken);

    Task<UsuarioResponse?> CriarAsync(
        CriarUsuarioRequest request,
        CancellationToken cancellationToken);

    Task<UsuarioResponse?> AtualizarAsync(
        string usuarioId,
        AtualizarUsuarioRequest request,
        CancellationToken cancellationToken);

    Task<bool> DesativarAsync(
        string usuarioId,
        CancellationToken cancellationToken);
}