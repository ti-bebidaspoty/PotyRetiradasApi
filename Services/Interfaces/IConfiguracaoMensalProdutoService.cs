using PotyRetiradasApi.Dtos.ConfiguracoesMensaisProdutos;

namespace PotyRetiradasApi.Services.Interfaces;

public interface IConfiguracaoMensalProdutoService
{
    Task<IReadOnlyList<ConfiguracaoMensalProdutoResponse>> ListarAsync(
        CancellationToken cancellationToken);

    Task<ConfiguracaoMensalProdutoResponse?> ObterPorIdAsync(
        string configuracaoMensalId,
        CancellationToken cancellationToken);

    Task<ConfiguracaoMensalProdutoResponse?> CriarAsync(
        CriarConfiguracaoMensalProdutoRequest request,
        CancellationToken cancellationToken);

    Task<ConfiguracaoMensalProdutoResponse?> AtualizarAsync(
        string configuracaoMensalId,
        AtualizarConfiguracaoMensalProdutoRequest request,
        CancellationToken cancellationToken);

    Task<bool> ExcluirAsync(
        string configuracaoMensalId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ProdutoConfiguradoMesResponse>> ListarProdutosConfiguradosAsync(
    string anoMes,
    int unidadeId,
    CancellationToken cancellationToken);
}