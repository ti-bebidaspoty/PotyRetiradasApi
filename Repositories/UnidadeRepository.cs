using Microsoft.EntityFrameworkCore;
using PotyRetiradasApi.Data;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;

namespace PotyRetiradasApi.Repositories;

public sealed class UnidadeRepository : IUnidadeRepository
{
    private readonly RetiradasDbContext _context;

    public UnidadeRepository(RetiradasDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Unidade>> ListarAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Unidades
            .AsNoTracking()
            .OrderBy(unidade => unidade.UnidadeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Unidade?> ObterPorIdAsync(
        int unidadeId,
        bool rastrear,
        CancellationToken cancellationToken)
    {
        IQueryable<Unidade> consulta = _context.Unidades;

        if (!rastrear)
        {
            consulta = consulta.AsNoTracking();
        }

        return await consulta.FirstOrDefaultAsync(
            unidade => unidade.UnidadeId == unidadeId,
            cancellationToken);
    }

    public async Task<bool> ExisteAsync(
        int unidadeId,
        CancellationToken cancellationToken)
    {
        return await _context.Unidades.AnyAsync(
            unidade => unidade.UnidadeId == unidadeId,
            cancellationToken);
    }

    public async Task AdicionarAsync(
        Unidade unidade,
        CancellationToken cancellationToken)
    {
        await _context.Unidades.AddAsync(
            unidade,
            cancellationToken);
    }

    public void Remover(Unidade unidade)
    {
        _context.Unidades.Remove(unidade);
    }

    public async Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}