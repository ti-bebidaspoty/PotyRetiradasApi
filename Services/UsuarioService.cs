using Microsoft.AspNetCore.Identity;
using PotyRetiradasApi.Dtos.Usuarios;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Services;

public sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnidadeRepository _unidadeRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IUnidadeRepository unidadeRepository,
        IPasswordHasher<Usuario> passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _unidadeRepository = unidadeRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<IReadOnlyList<UsuarioResponse>> ListarAsync(
        CancellationToken cancellationToken)
    {
        var usuarios = await _usuarioRepository.ListarAsync(
            cancellationToken);

        return usuarios
            .Select(MapearResponse)
            .ToList();
    }

    public async Task<UsuarioResponse?> ObterPorIdAsync(
        string usuarioId,
        CancellationToken cancellationToken)
    {
        usuarioId = NormalizarId(usuarioId);

        var usuario = await _usuarioRepository.ObterPorIdAsync(
            usuarioId,
            rastrear: false,
            cancellationToken);

        return usuario is null
            ? null
            : MapearResponse(usuario);
    }

    public async Task<UsuarioResponse?> CriarAsync(
        CriarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var login = NormalizarLogin(request.Usuario);

        var loginJaExiste = await _usuarioRepository.ExisteLoginAsync(
            login,
            ignorarUsuarioId: null,
            cancellationToken);

        if (loginJaExiste)
        {
            return null;
        }

        var unidadeExiste = await _unidadeRepository.ExisteAsync(
            request.UnidadeId,
            cancellationToken);

        if (!unidadeExiste)
        {
            throw new ArgumentException(
                $"Unidade {request.UnidadeId} não encontrada.");
        }

        var usuario = new Usuario
        {
            UsuarioId = Guid.NewGuid().ToString(),
            Nome = request.Nome.Trim(),
            Usuario1 = login,
            Administrador = request.Administrador,
            Status = request.Status,
            UnidadeId = request.UnidadeId,
            SenhaHash = string.Empty
        };

        usuario.SenhaHash = _passwordHasher.HashPassword(
            usuario,
            request.Senha);

        await _usuarioRepository.AdicionarAsync(
            usuario,
            cancellationToken);

        await _usuarioRepository.SalvarAlteracoesAsync(
            cancellationToken);

        return MapearResponse(usuario);
    }

    public async Task<UsuarioResponse?> AtualizarAsync(
        string usuarioId,
        AtualizarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        usuarioId = NormalizarId(usuarioId);

        var usuario = await _usuarioRepository.ObterPorIdAsync(
            usuarioId,
            rastrear: true,
            cancellationToken);

        if (usuario is null)
        {
            return null;
        }

        var login = NormalizarLogin(request.Usuario);

        var loginJaExiste = await _usuarioRepository.ExisteLoginAsync(
            login,
            ignorarUsuarioId: usuarioId,
            cancellationToken);

        if (loginJaExiste)
        {
            throw new ArgumentException(
                $"Já existe outro usuário com o login {login}.");
        }

        var unidadeExiste = await _unidadeRepository.ExisteAsync(
            request.UnidadeId,
            cancellationToken);

        if (!unidadeExiste)
        {
            throw new ArgumentException(
                $"Unidade {request.UnidadeId} não encontrada.");
        }

        usuario.Nome = request.Nome.Trim();
        usuario.Usuario1 = login;
        usuario.Administrador = request.Administrador;
        usuario.Status = request.Status;
        usuario.UnidadeId = request.UnidadeId;

        if (!string.IsNullOrWhiteSpace(request.Senha))
        {
            usuario.SenhaHash = _passwordHasher.HashPassword(
                usuario,
                request.Senha);
        }

        await _usuarioRepository.SalvarAlteracoesAsync(
            cancellationToken);

        return MapearResponse(usuario);
    }

    public async Task<bool> DesativarAsync(
        string usuarioId,
        CancellationToken cancellationToken)
    {
        usuarioId = NormalizarId(usuarioId);

        var usuario = await _usuarioRepository.ObterPorIdAsync(
            usuarioId,
            rastrear: true,
            cancellationToken);

        if (usuario is null)
        {
            return false;
        }

        usuario.Status = false;

        await _usuarioRepository.SalvarAlteracoesAsync(
            cancellationToken);

        return true;
    }

    private static UsuarioResponse MapearResponse(
        Usuario usuario)
    {
        return new UsuarioResponse
        {
            UsuarioId = usuario.UsuarioId,
            Nome = usuario.Nome,
            Usuario = usuario.Usuario1,
            Administrador = usuario.Administrador,
            Status = usuario.Status,
            UnidadeId = usuario.UnidadeId
        };
    }

    private static string NormalizarId(string id)
    {
        return id.Trim();
    }

    private static string NormalizarLogin(string login)
    {
        return login.Trim().ToLowerInvariant();
    }
}