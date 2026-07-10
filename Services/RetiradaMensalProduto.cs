using PotyRetiradasApi.Dtos.RetiradasMensais;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Services;

public sealed class RetiradaMensalProdutoService
    : IRetiradaMensalProdutoService
{
    private readonly IRetiradaMensalProdutoRepository _retiradaMensalProdutoRepository;
    private readonly IRetiradaMensalRepository _retiradaMensalRepository;
    private readonly IColaboradorRepository _colaboradorRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IConfiguracaoMensalProdutoRepository _configuracaoRepository;

    public RetiradaMensalProdutoService(
        IRetiradaMensalProdutoRepository retiradaMensalProdutoRepository,
        IRetiradaMensalRepository retiradaMensalRepository,
        IColaboradorRepository colaboradorRepository,
        IProdutoRepository produtoRepository,
        IConfiguracaoMensalProdutoRepository configuracaoRepository)
    {
        _retiradaMensalProdutoRepository = retiradaMensalProdutoRepository;
        _retiradaMensalRepository = retiradaMensalRepository;
        _colaboradorRepository = colaboradorRepository;
        _produtoRepository = produtoRepository;
        _configuracaoRepository = configuracaoRepository;
    }

    public async Task<ProdutoRetiradaResponse> AdicionarAsync(
        string retiradaMensalId,
        ProdutoRetiradaRequest request,
        CancellationToken cancellationToken)
    {
        retiradaMensalId = NormalizarId(retiradaMensalId);

        var produtoId = NormalizarProdutoId(
            request.ProdutoId);

        ValidarAdicionarRequest(
            retiradaMensalId,
            produtoId,
            request.Quantidade);

        // 1. Verifica se a retirada existe
        var retirada = await _retiradaMensalRepository.ObterPorIdAsync(
            retiradaMensalId,
            rastrear: false,
            cancellationToken);

        if (retirada is null)
        {
            throw new ArgumentException(
                $"Retirada mensal {retiradaMensalId} não encontrada.");
        }

        // 2. Impede produto duplicado na mesma retirada
        var produtoJaExisteNaRetirada =
            await _retiradaMensalProdutoRepository.ExisteProdutoNaRetiradaAsync(
                retiradaMensalId,
                produtoId,
                cancellationToken);

        if (produtoJaExisteNaRetirada)
        {
            throw new ArgumentException(
                $"O produto {produtoId} já existe nesta retirada.");
        }

        // 3. Verifica se o produto existe
        var produtoExiste = await _produtoRepository.ExisteAsync(
            produtoId,
            cancellationToken);

        if (!produtoExiste)
        {
            throw new ArgumentException(
                $"Produto {produtoId} não encontrado.");
        }

        // 4. Busca a configuração mensal do produto
        var configuracao = await _configuracaoRepository.ObterPorChaveAsync(
            retirada.AnoMes,
            retirada.UnidadeId,
            produtoId,
            cancellationToken);

        if (configuracao is null)
        {
            throw new ArgumentException(
                $"Não existe configuração para o produto {produtoId} " +
                $"no mês {retirada.AnoMes} e unidade {retirada.UnidadeId}.");
        }

        // 5. Valida quantidade mínima
        if (request.Quantidade < configuracao.Minimo)
        {
            throw new ArgumentException(
                $"A quantidade do produto {produtoId} não pode ser menor " +
                $"que o mínimo configurado ({configuracao.Minimo}).");
        }

        // 6. Valida quantidade máxima
        if (request.Quantidade > configuracao.Maximo)
        {
            throw new ArgumentException(
                $"A quantidade do produto {produtoId} não pode ser maior " +
                $"que o máximo configurado ({configuracao.Maximo}).");
        }

        // 7. Busca o colaborador para validar o limite do tipo
        var colaborador = await _colaboradorRepository.ObterPorIdAsync(
            retirada.ColaboradorId,
            rastrear: false,
            cancellationToken);

        if (colaborador is null)
        {
            throw new ArgumentException(
                $"Colaborador {retirada.ColaboradorId} não encontrado.");
        }

        if (colaborador.Tipo is null)
        {
            throw new ArgumentException(
                "O colaborador informado não possui tipo vinculado.");
        }

        // 8. Busca os produtos que já existem na retirada
        var produtosAtuais =
            await _retiradaMensalProdutoRepository.ListarPorRetiradaMensalIdAsync(
                retiradaMensalId,
                cancellationToken);

        var quantidadeAtual = produtosAtuais.Sum(
            item => item.Quantidade);

        var novaQuantidadeTotal =
            quantidadeAtual + request.Quantidade;

        // 9. Valida o limite total do tipo do colaborador
        if (novaQuantidadeTotal > colaborador.Tipo.QuantidadeRetirada)
        {
            throw new ArgumentException(
                $"A quantidade total retirada ({novaQuantidadeTotal}) " +
                $"excede o limite do tipo {colaborador.Tipo.Descricao} " +
                $"({colaborador.Tipo.QuantidadeRetirada}).");
        }

        // 10. Cria o novo item
        var novoProduto = new RetiradasMensaisProduto
        {
            RetiradaMensalProdutoId = Guid.NewGuid().ToString(),
            RetiradaMensalId = retiradaMensalId,
            ProdutoId = produtoId,
            Quantidade = request.Quantidade
        };

        // 11. Adiciona no repositório
        await _retiradaMensalProdutoRepository.AdicionarAsync(
            novoProduto,
            cancellationToken);

        // 12. Persiste no banco
        await _retiradaMensalProdutoRepository.SalvarAlteracoesAsync(
            cancellationToken);

        // 13. Busca novamente para retornar com descrição do produto
        var produtosAtualizados =
            await _retiradaMensalProdutoRepository.ListarPorRetiradaMensalIdAsync(
                retiradaMensalId,
                cancellationToken);

        var produtoCriado = produtosAtualizados.FirstOrDefault(
            item =>
                item.RetiradaMensalProdutoId ==
                novoProduto.RetiradaMensalProdutoId);

        return produtoCriado is null
            ? MapearResponse(novoProduto)
            : MapearResponse(produtoCriado);
    }

    public async Task<bool> ExcluirAsync(
        string retiradaMensalProdutoId,
        CancellationToken cancellationToken)
    {
        retiradaMensalProdutoId = NormalizarId(
            retiradaMensalProdutoId);

        if (string.IsNullOrWhiteSpace(retiradaMensalProdutoId))
        {
            throw new ArgumentException(
                "O ID do produto da retirada é obrigatório.");
        }

        var excluido =
            await _retiradaMensalProdutoRepository.ExcluirPorIdAsync(
                retiradaMensalProdutoId,
                cancellationToken);

        if (!excluido)
        {
            return false;
        }

        await _retiradaMensalProdutoRepository.SalvarAlteracoesAsync(
            cancellationToken);

        return true;
    }

    private static ProdutoRetiradaResponse MapearResponse(
        RetiradasMensaisProduto item)
    {
        return new ProdutoRetiradaResponse
        {
            RetiradaMensalProdutoId =
                item.RetiradaMensalProdutoId,

            ProdutoId =
                item.ProdutoId,

            ProdutoDescricao =
                item.Produto?.Descricao,

            Quantidade =
                item.Quantidade
        };
    }

    private static void ValidarAdicionarRequest(
        string retiradaMensalId,
        string produtoId,
        int quantidade)
    {
        if (string.IsNullOrWhiteSpace(retiradaMensalId))
        {
            throw new ArgumentException(
                "O ID da retirada mensal é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(produtoId))
        {
            throw new ArgumentException(
                "O ID do produto é obrigatório.");
        }

        if (produtoId.Length > 8)
        {
            throw new ArgumentException(
                "O ID do produto deve possuir no máximo 8 caracteres.");
        }

        if (quantidade <= 0)
        {
            throw new ArgumentException(
                $"A quantidade do produto {produtoId} deve ser maior que zero.");
        }
    }

    private static string NormalizarId(
        string id)
    {
        return id?.Trim() ?? string.Empty;
    }

    private static string NormalizarProdutoId(
        string produtoId)
    {
        return produtoId?
            .Trim()
            .ToUpperInvariant()
            ?? string.Empty;
    }
}