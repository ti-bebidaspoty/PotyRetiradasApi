using Microsoft.EntityFrameworkCore;
using PotyRetiradasApi.Data;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;

namespace PotyRetiradasApi.Repositories;

public sealed class ProdutoRepository : IProdutoRepository
{
    private readonly RetiradasDbContext _context;

    public ProdutoRepository(RetiradasDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Produto>> ListarAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Produtos
            .AsNoTracking()
            .OrderBy(produto => produto.Descricao)
            .ToListAsync(cancellationToken);
    }

    public async Task<Produto?> ObterPorIdAsync(
        string produtoId,
        bool rastrear,
        CancellationToken cancellationToken)
    {
        IQueryable<Produto> consulta = _context.Produtos;

        if (!rastrear)
        {
            consulta = consulta.AsNoTracking();
        }

        return await consulta.FirstOrDefaultAsync(
            produto => produto.ProdutoId == produtoId,
            cancellationToken);
    }

    public async Task<bool> ExisteAsync(
        string produtoId,
        CancellationToken cancellationToken)
    {
        return await _context.Produtos.AnyAsync(
            produto => produto.ProdutoId == produtoId,
            cancellationToken);
    }

    public async Task AdicionarAsync(
        Produto produto,
        CancellationToken cancellationToken)
    {
        await _context.Produtos.AddAsync(
            produto,
            cancellationToken);
    }

    public async Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(
            cancellationToken);
    }
}
