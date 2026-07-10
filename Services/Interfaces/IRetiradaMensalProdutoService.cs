using PotyRetiradasApi.Dtos.RetiradasMensais;

namespace PotyRetiradasApi.Services.Interfaces;

public interface IRetiradaMensalProdutoService
{
    Task<ProdutoRetiradaResponse> AdicionarAsync(
        string retiradaMensalId,
        ProdutoRetiradaRequest request,
        CancellationToken cancellationToken);

    Task<bool> ExcluirAsync(
        string retiradaMensalProdutoId,
        CancellationToken cancellationToken);
}