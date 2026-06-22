using Microsoft.EntityFrameworkCore;
using PotyRetiradasApi.Data;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;

namespace PotyRetiradasApi.Repositories;

public sealed class TipoRepository : ITipoRepository
{
    private readonly RetiradasDbContext _context;

    public TipoRepository(RetiradasDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Tipo>> ListarAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Tipos
            .AsNoTracking()
            .OrderBy(tipo => tipo.Descricao)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tipo?> ObterPorIdAsync(
        string tipoId,
        bool rastrear,
        CancellationToken cancellationToken)
    {
        IQueryable<Tipo> consulta = _context.Tipos;

        if (!rastrear)
        {
            consulta = consulta.AsNoTracking();
        }

        return await consulta.FirstOrDefaultAsync(
            tipo => tipo.TipoId == tipoId,
            cancellationToken);
    }

    public async Task<bool> ExisteAsync(
        string tipoId,
        CancellationToken cancellationToken)
    {
        return await _context.Tipos.AnyAsync(
            tipo => tipo.TipoId == tipoId,
            cancellationToken);
    }

    public async Task AdicionarAsync(
        Tipo tipo,
        CancellationToken cancellationToken)
    {
        await _context.Tipos.AddAsync(tipo, cancellationToken);
    }

    public void Remover(Tipo tipo)
    {
        _context.Tipos.Remove(tipo);
    }

    public async Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}