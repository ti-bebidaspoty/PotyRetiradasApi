using PotyRetiradasApi.Dtos.ConfiguracoesMensaisProdutos;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Services;

public sealed class ConfiguracaoMensalProdutoService
    : IConfiguracaoMensalProdutoService
{
    private readonly IConfiguracaoMensalProdutoRepository _repository;
    private readonly IUnidadeRepository _unidadeRepository;
    private readonly IProdutoRepository _produtoRepository;

    public ConfiguracaoMensalProdutoService(
        IConfiguracaoMensalProdutoRepository repository,
        IUnidadeRepository unidadeRepository,
        IProdutoRepository produtoRepository)
    {
        _repository = repository;
        _unidadeRepository = unidadeRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<IReadOnlyList<ConfiguracaoMensalProdutoResponse>> ListarAsync(
        CancellationToken cancellationToken)
    {
        var configuracoes = await _repository.ListarAsync(
            cancellationToken);

        return configuracoes
            .Select(MapearResponse)
            .ToList();
    }

    public async Task<ConfiguracaoMensalProdutoResponse?> ObterPorIdAsync(
        string configuracaoMensalId,
        CancellationToken cancellationToken)
    {
        configuracaoMensalId = NormalizarId(configuracaoMensalId);

        var configuracao = await _repository.ObterPorIdAsync(
            configuracaoMensalId,
            rastrear: false,
            cancellationToken);

        return configuracao is null
            ? null
            : MapearResponse(configuracao);
    }

    public async Task<ConfiguracaoMensalProdutoResponse?> CriarAsync(
        CriarConfiguracaoMensalProdutoRequest request,
        CancellationToken cancellationToken)
    {
        var anoMes = NormalizarAnoMes(request.AnoMes);
        var produtoId = NormalizarProdutoId(request.ProdutoId);

        ValidarAnoMes(anoMes);
        ValidarLimites(request.Minimo, request.Maximo);

        await ValidarUnidadeExisteAsync(
            request.UnidadeId,
            cancellationToken);

        await ValidarProdutoExisteAsync(
            produtoId,
            cancellationToken);

        var jaExiste = await _repository.ExisteConfiguracaoAsync(
            anoMes,
            request.UnidadeId,
            produtoId,
            ignorarConfiguracaoMensalId: null,
            cancellationToken);

        if (jaExiste)
        {
            return null;
        }

        var configuracao = new ConfiguracoesMensaisProduto
        {
            ConfiguracaoMensalId = Guid.NewGuid().ToString(),
            AnoMes = anoMes,
            UnidadeId = request.UnidadeId,
            ProdutoId = produtoId,
            Minimo = request.Minimo,
            Maximo = request.Maximo
        };

        await _repository.AdicionarAsync(
            configuracao,
            cancellationToken);

        await _repository.SalvarAlteracoesAsync(
            cancellationToken);

        var configuracaoCriada = await _repository.ObterPorIdAsync(
            configuracao.ConfiguracaoMensalId,
            rastrear: false,
            cancellationToken);

        return configuracaoCriada is null
            ? MapearResponse(configuracao)
            : MapearResponse(configuracaoCriada);
    }

    public async Task<ConfiguracaoMensalProdutoResponse?> AtualizarAsync(
        string configuracaoMensalId,
        AtualizarConfiguracaoMensalProdutoRequest request,
        CancellationToken cancellationToken)
    {
        configuracaoMensalId = NormalizarId(configuracaoMensalId);

        var configuracao = await _repository.ObterPorIdAsync(
            configuracaoMensalId,
            rastrear: true,
            cancellationToken);

        if (configuracao is null)
        {
            return null;
        }

        var anoMes = NormalizarAnoMes(request.AnoMes);
        var produtoId = NormalizarProdutoId(request.ProdutoId);

        ValidarAnoMes(anoMes);
        ValidarLimites(request.Minimo, request.Maximo);

        await ValidarUnidadeExisteAsync(
            request.UnidadeId,
            cancellationToken);

        await ValidarProdutoExisteAsync(
            produtoId,
            cancellationToken);

        var jaExiste = await _repository.ExisteConfiguracaoAsync(
            anoMes,
            request.UnidadeId,
            produtoId,
            ignorarConfiguracaoMensalId: configuracaoMensalId,
            cancellationToken);

        if (jaExiste)
        {
            throw new ArgumentException(
                "Já existe uma configuração para este mês, unidade e produto.");
        }

        configuracao.AnoMes = anoMes;
        configuracao.UnidadeId = request.UnidadeId;
        configuracao.ProdutoId = produtoId;
        configuracao.Minimo = request.Minimo;
        configuracao.Maximo = request.Maximo;

        await _repository.SalvarAlteracoesAsync(
            cancellationToken);

        var configuracaoAtualizada = await _repository.ObterPorIdAsync(
            configuracaoMensalId,
            rastrear: false,
            cancellationToken);

        return configuracaoAtualizada is null
            ? MapearResponse(configuracao)
            : MapearResponse(configuracaoAtualizada);
    }

    public async Task<bool> ExcluirAsync(
        string configuracaoMensalId,
        CancellationToken cancellationToken)
    {
        configuracaoMensalId = NormalizarId(configuracaoMensalId);

        var configuracao = await _repository.ObterPorIdAsync(
            configuracaoMensalId,
            rastrear: true,
            cancellationToken);

        if (configuracao is null)
        {
            return false;
        }

        _repository.Remover(configuracao);

        await _repository.SalvarAlteracoesAsync(
            cancellationToken);

        return true;
    }

    private async Task ValidarUnidadeExisteAsync(
        int unidadeId,
        CancellationToken cancellationToken)
    {
        var unidadeExiste = await _unidadeRepository.ExisteAsync(
            unidadeId,
            cancellationToken);

        if (!unidadeExiste)
        {
            throw new ArgumentException(
                $"Unidade {unidadeId} não encontrada.");
        }
    }

    private async Task ValidarProdutoExisteAsync(
        string produtoId,
        CancellationToken cancellationToken)
    {
        var produtoExiste = await _produtoRepository.ExisteAsync(
            produtoId,
            cancellationToken);

        if (!produtoExiste)
        {
            throw new ArgumentException(
                $"Produto {produtoId} não encontrado.");
        }
    }

    private static ConfiguracaoMensalProdutoResponse MapearResponse(
        ConfiguracoesMensaisProduto configuracao)
    {
        return new ConfiguracaoMensalProdutoResponse
        {
            ConfiguracaoMensalId = configuracao.ConfiguracaoMensalId,
            AnoMes = configuracao.AnoMes,
            UnidadeId = configuracao.UnidadeId,
            UnidadeDescricao = configuracao.Unidade?.Descricao,
            ProdutoId = configuracao.ProdutoId,
            ProdutoDescricao = configuracao.Produto?.Descricao,
            Minimo = configuracao.Minimo,
            Maximo = configuracao.Maximo
        };
    }

    private static string NormalizarId(
        string id)
    {
        return id.Trim();
    }

    private static string NormalizarAnoMes(
        string anoMes)
    {
        return anoMes.Trim();
    }

    private static string NormalizarProdutoId(
        string produtoId)
    {
        return produtoId.Trim().ToUpperInvariant();
    }

    private static void ValidarLimites(
        int minimo,
        int maximo)
    {
        if (minimo < 0)
        {
            throw new ArgumentException(
                "O mínimo não pode ser negativo.");
        }

        if (maximo < 0)
        {
            throw new ArgumentException(
                "O máximo não pode ser negativo.");
        }

        if (maximo < minimo)
        {
            throw new ArgumentException(
                "O máximo não pode ser menor que o mínimo.");
        }
    }

    private static void ValidarAnoMes(
        string anoMes)
    {
        if (anoMes.Length != 6)
        {
            throw new ArgumentException(
                "O ano/mês deve estar no formato yyyyMM.");
        }

        if (!int.TryParse(anoMes[..4], out var ano))
        {
            throw new ArgumentException(
                "O ano do campo AnoMes é inválido.");
        }

        if (!int.TryParse(anoMes[4..6], out var mes))
        {
            throw new ArgumentException(
                "O mês do campo AnoMes é inválido.");
        }

        if (ano < 2000 || ano > 2100)
        {
            throw new ArgumentException(
                "O ano do campo AnoMes deve estar entre 2000 e 2100.");
        }

        if (mes < 1 || mes > 12)
        {
            throw new ArgumentException(
                "O mês do campo AnoMes deve estar entre 01 e 12.");
        }
    }

    public async Task<IReadOnlyList<ProdutoConfiguradoMesResponse>> ListarProdutosConfiguradosAsync(
    string anoMes,
    int unidadeId,
    CancellationToken cancellationToken)
    {
        anoMes = NormalizarAnoMes(anoMes);

        ValidarAnoMes(anoMes);

        var unidadeExiste = await _unidadeRepository.ExisteAsync(
            unidadeId,
            cancellationToken);

        if (!unidadeExiste)
        {
            throw new ArgumentException(
                $"Unidade {unidadeId} não encontrada.");
        }

        var configuracoes = await _repository.ListarProdutosConfiguradosAsync(
            anoMes,
            unidadeId,
            cancellationToken);

        return configuracoes
            .Select(configuracao => new ProdutoConfiguradoMesResponse
            {
                ConfiguracaoMensalId = configuracao.ConfiguracaoMensalId,
                ProdutoId = configuracao.ProdutoId,
                ProdutoDescricao = configuracao.Produto?.Descricao ?? string.Empty,
                CodigoBarras = configuracao.Produto?.CodigoBarras,
                Imagem = configuracao.Produto?.Imagem,
                Minimo = configuracao.Minimo,
                Maximo = configuracao.Maximo
            })
            .ToList();
    }
}