using Microsoft.EntityFrameworkCore;
using PotyRetiradasApi.Data;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;

namespace PotyRetiradasApi.Repositories;

public sealed class RetiradaMensalRepository : IRetiradaMensalRepository
{
    private readonly RetiradasDbContext _context;

    public RetiradaMensalRepository(RetiradasDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<RetiradasMensai>> ListarAsync(
        CancellationToken cancellationToken)
    {
        return await _context.RetiradasMensais
            .AsNoTracking()
            .Include(retirada => retirada.Unidade)
            .Include(retirada => retirada.Colaborador)
            .Include(retirada => retirada.Usuario)
            .Include(retirada => retirada.RetiradasMensaisProdutos)
                .ThenInclude(item => item.Produto)
            .OrderByDescending(retirada => retirada.DataHora)
            .ToListAsync(cancellationToken);
    }

    public async Task<RetiradasMensai?> ObterPorIdAsync(
        string retiradaMensalId,
        bool rastrear,
        CancellationToken cancellationToken)
    {
        IQueryable<RetiradasMensai> consulta = _context.RetiradasMensais
            .Include(retirada => retirada.Unidade)
            .Include(retirada => retirada.Colaborador)
            .Include(retirada => retirada.Usuario)
            .Include(retirada => retirada.RetiradasMensaisProdutos)
                .ThenInclude(item => item.Produto);

        if (!rastrear)
        {
            consulta = consulta.AsNoTracking();
        }

        return await consulta.FirstOrDefaultAsync(
            retirada => retirada.RetiradaMensalId == retiradaMensalId,
            cancellationToken);
    }

    public async Task AdicionarAsync(
        RetiradasMensai retirada,
        CancellationToken cancellationToken)
    {
        await _context.RetiradasMensais.AddAsync(
            retirada,
            cancellationToken);
    }

    public void Remover(
        RetiradasMensai retirada)
    {
        _context.RetiradasMensaisProdutos.RemoveRange(
            retirada.RetiradasMensaisProdutos);

        _context.RetiradasMensais.Remove(retirada);
    }

    public async Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExisteRetiradaDoColaboradorNoMesAsync(
    string anoMes,
    string colaboradorId,
    CancellationToken cancellationToken)
    {
        return await _context.RetiradasMensais.AnyAsync(
            retirada =>
                retirada.AnoMes == anoMes &&
                retirada.ColaboradorId == colaboradorId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<RetiradasMensai>> ListarPorAnoMesAsync(
    string anoMes,
    CancellationToken cancellationToken)
    {
        return await _context.RetiradasMensais
            .AsNoTracking()
            .Include(retirada => retirada.Unidade)
            .Include(retirada => retirada.Colaborador)
            .Include(retirada => retirada.Usuario)
            .Include(retirada => retirada.RetiradasMensaisProdutos)
                .ThenInclude(item => item.Produto)
            .Where(retirada => retirada.AnoMes == anoMes)
            .OrderByDescending(retirada => retirada.DataHora)
            .ToListAsync(cancellationToken);
    }
}