using Microsoft.EntityFrameworkCore;
using PotyRetiradasApi.Data;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;

namespace PotyRetiradasApi.Repositories;

public sealed class ConfiguracaoMensalProdutoRepository
    : IConfiguracaoMensalProdutoRepository
{
    private readonly RetiradasDbContext _context;

    public ConfiguracaoMensalProdutoRepository(
        RetiradasDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ConfiguracoesMensaisProduto>> ListarAsync(
        CancellationToken cancellationToken)
    {
        return await _context.ConfiguracoesMensaisProdutos
            .AsNoTracking()
            .Include(configuracao => configuracao.Unidade)
            .Include(configuracao => configuracao.Produto)
            .OrderByDescending(configuracao => configuracao.AnoMes)
            .ThenBy(configuracao => configuracao.Unidade.Descricao)
            .ThenBy(configuracao => configuracao.Produto.Descricao)
            .ToListAsync(cancellationToken);
    }

    public async Task<ConfiguracoesMensaisProduto?> ObterPorIdAsync(
        string configuracaoMensalId,
        bool rastrear,
        CancellationToken cancellationToken)
    {
        IQueryable<ConfiguracoesMensaisProduto> consulta =
            _context.ConfiguracoesMensaisProdutos
                .Include(configuracao => configuracao.Unidade)
                .Include(configuracao => configuracao.Produto);

        if (!rastrear)
        {
            consulta = consulta.AsNoTracking();
        }

        return await consulta.FirstOrDefaultAsync(
            configuracao =>
                configuracao.ConfiguracaoMensalId == configuracaoMensalId,
            cancellationToken);
    }

    public async Task<bool> ExisteConfiguracaoAsync(
        string anoMes,
        int unidadeId,
        string produtoId,
        string? ignorarConfiguracaoMensalId,
        CancellationToken cancellationToken)
    {
        return await _context.ConfiguracoesMensaisProdutos.AnyAsync(
            configuracao =>
                configuracao.AnoMes == anoMes &&
                configuracao.UnidadeId == unidadeId &&
                configuracao.ProdutoId == produtoId &&
                (
                    ignorarConfiguracaoMensalId == null ||
                    configuracao.ConfiguracaoMensalId != ignorarConfiguracaoMensalId
                ),
            cancellationToken);
    }

    public async Task AdicionarAsync(
        ConfiguracoesMensaisProduto configuracao,
        CancellationToken cancellationToken)
    {
        await _context.ConfiguracoesMensaisProdutos.AddAsync(
            configuracao,
            cancellationToken);
    }

    public void Remover(
        ConfiguracoesMensaisProduto configuracao)
    {
        _context.ConfiguracoesMensaisProdutos.Remove(configuracao);
    }

    public async Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ConfiguracoesMensaisProduto?> ObterPorChaveAsync(
    string anoMes,
    int unidadeId,
    string produtoId,
    CancellationToken cancellationToken)
    {
        return await _context.ConfiguracoesMensaisProdutos
            .AsNoTracking()
            .FirstOrDefaultAsync(
                configuracao =>
                    configuracao.AnoMes == anoMes &&
                    configuracao.UnidadeId == unidadeId &&
                    configuracao.ProdutoId == produtoId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<ConfiguracoesMensaisProduto>> ListarProdutosConfiguradosAsync(
    string anoMes,
    int unidadeId,
    CancellationToken cancellationToken)
    {
        return await _context.ConfiguracoesMensaisProdutos
            .AsNoTracking()
            .Include(configuracao => configuracao.Produto)
            .Where(configuracao =>
                configuracao.AnoMes == anoMes &&
                configuracao.UnidadeId == unidadeId &&
                configuracao.Produto.Status)
            .OrderBy(configuracao => configuracao.Produto.Descricao)
            .ToListAsync(cancellationToken);
    }
}