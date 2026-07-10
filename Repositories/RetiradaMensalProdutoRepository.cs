using Microsoft.EntityFrameworkCore;
using PotyRetiradasApi.Data;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;

namespace PotyRetiradasApi.Repositories;

public sealed class RetiradaMensalProdutoRepository
    : IRetiradaMensalProdutoRepository
{
    private readonly RetiradasDbContext _context;

    public RetiradaMensalProdutoRepository(
        RetiradasDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<RetiradasMensaisProduto>> ListarPorRetiradaMensalIdAsync(
        string retiradaMensalId,
        CancellationToken cancellationToken)
    {
        return await _context.RetiradasMensaisProdutos
            .AsNoTracking()
            .Include(item => item.Produto)
            .Where(item => item.RetiradaMensalId == retiradaMensalId)
            .OrderBy(item => item.Produto.Descricao)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarVariosAsync(
        IEnumerable<RetiradasMensaisProduto> produtos,
        CancellationToken cancellationToken)
    {
        await _context.RetiradasMensaisProdutos.AddRangeAsync(
            produtos,
            cancellationToken);
    }

    public void RemoverVarios(
        IEnumerable<RetiradasMensaisProduto> produtos)
    {
        _context.RetiradasMensaisProdutos.RemoveRange(produtos);
    }

    public async Task<bool> ExisteProdutoNaRetiradaAsync(
        string retiradaMensalId,
        string produtoId,
        CancellationToken cancellationToken)
    {
        return await _context.RetiradasMensaisProdutos.AnyAsync(
            item =>
                item.RetiradaMensalId == retiradaMensalId &&
                item.ProdutoId == produtoId,
            cancellationToken);
    }

    public async Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RetiradasMensaisProduto> AdicionarAsync(
    RetiradasMensaisProduto produto,
    CancellationToken cancellationToken)
    {
        await _context.RetiradasMensaisProdutos.AddAsync(
            produto,
            cancellationToken);

        return produto;
    }

    public async Task<bool> ExcluirPorIdAsync(
    string retiradaMensalProdutoId,
    CancellationToken cancellationToken)
    {
        var produto = await _context.RetiradasMensaisProdutos
            .FirstOrDefaultAsync(
                item =>
                    item.RetiradaMensalProdutoId == retiradaMensalProdutoId,
                cancellationToken);

        if (produto is null)
        {
            return false;
        }

        _context.RetiradasMensaisProdutos.Remove(produto);

        return true;
    }
}