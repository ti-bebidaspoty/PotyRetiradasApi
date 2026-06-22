using PotyRetiradasApi.Entities;

namespace PotyRetiradasApi.Repositories.Interfaces;

public interface IConfiguracaoMensalProdutoRepository
{
    Task<IReadOnlyList<ConfiguracoesMensaisProduto>> ListarAsync(
        CancellationToken cancellationToken);

    Task<ConfiguracoesMensaisProduto?> ObterPorIdAsync(
        string configuracaoMensalId,
        bool rastrear,
        CancellationToken cancellationToken);

    Task<bool> ExisteConfiguracaoAsync(
        string anoMes,
        int unidadeId,
        string produtoId,
        string? ignorarConfiguracaoMensalId,
        CancellationToken cancellationToken);

    Task AdicionarAsync(
        ConfiguracoesMensaisProduto configuracao,
        CancellationToken cancellationToken);

    void Remover(
        ConfiguracoesMensaisProduto configuracao);

    Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken);

    Task<ConfiguracoesMensaisProduto?> ObterPorChaveAsync(
    string anoMes,
    int unidadeId,
    string produtoId,
    CancellationToken cancellationToken);

    Task<IReadOnlyList<ConfiguracoesMensaisProduto>> ListarProdutosConfiguradosAsync(
    string anoMes,
    int unidadeId,
    CancellationToken cancellationToken);
}