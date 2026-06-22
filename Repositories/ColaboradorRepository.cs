using Microsoft.EntityFrameworkCore;
using PotyRetiradasApi.Data;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;

namespace PotyRetiradasApi.Repositories;

public sealed class ColaboradorRepository : IColaboradorRepository
{
    private readonly RetiradasDbContext _context;

    public ColaboradorRepository(RetiradasDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Colaboradore>> ListarAsync(
    CancellationToken cancellationToken)
    {
        return await _context.Colaboradores
            .AsNoTracking()
            .Include(colaborador => colaborador.Tipo)
            .OrderBy(colaborador => colaborador.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Colaboradore?> ObterPorIdAsync(
        string colaboradorId,
        bool rastrear,
        CancellationToken cancellationToken)
    {
        IQueryable<Colaboradore> consulta = _context.Colaboradores
            .Include(colaborador => colaborador.Tipo);

        if (!rastrear)
        {
            consulta = consulta.AsNoTracking();
        }

        return await consulta.FirstOrDefaultAsync(
            colaborador => colaborador.ColaboradorId == colaboradorId,
            cancellationToken);
    }

    public async Task<bool> ExisteAsync(
        string colaboradorId,
        CancellationToken cancellationToken)
    {
        return await _context.Colaboradores.AnyAsync(
            colaborador => colaborador.ColaboradorId == colaboradorId,
            cancellationToken);
    }

    public async Task AdicionarAsync(
        Colaboradore colaborador,
        CancellationToken cancellationToken)
    {
        await _context.Colaboradores.AddAsync(
            colaborador,
            cancellationToken);
    }

    public async Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}