using Microsoft.EntityFrameworkCore;
using PotyRetiradasApi.Data;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;

namespace PotyRetiradasApi.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly RetiradasDbContext _context;

    public UsuarioRepository(RetiradasDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Usuario>> ListarAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .OrderBy(usuario => usuario.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Usuario?> ObterPorIdAsync(
        string usuarioId,
        bool rastrear,
        CancellationToken cancellationToken)
    {
        IQueryable<Usuario> consulta = _context.Usuarios;

        if (!rastrear)
        {
            consulta = consulta.AsNoTracking();
        }

        return await consulta.FirstOrDefaultAsync(
            usuario => usuario.UsuarioId == usuarioId,
            cancellationToken);
    }

    public async Task<bool> ExisteLoginAsync(
        string login,
        string? ignorarUsuarioId,
        CancellationToken cancellationToken)
    {
        return await _context.Usuarios.AnyAsync(
            usuario =>
                usuario.Usuario1 == login &&
                (ignorarUsuarioId == null || usuario.UsuarioId != ignorarUsuarioId),
            cancellationToken);
    }

    public async Task AdicionarAsync(
        Usuario usuario,
        CancellationToken cancellationToken)
    {
        await _context.Usuarios.AddAsync(
            usuario,
            cancellationToken);
    }

    public async Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Usuario?> ObterPorLoginAsync(
    string login,
    CancellationToken cancellationToken)
    {
        var loginNormalizado = login.Trim().ToLower();

        return await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(
                usuario =>
                    usuario.Usuario1 != null &&
                    usuario.Usuario1.Trim().ToLower() == loginNormalizado,
                cancellationToken);
    }
}