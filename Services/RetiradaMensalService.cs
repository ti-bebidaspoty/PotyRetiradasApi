using PotyRetiradasApi.Dtos.RetiradasMensais;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Services;

public sealed class RetiradaMensalService : IRetiradaMensalService
{
    private readonly IRetiradaMensalRepository _retiradaRepository;
    private readonly IUnidadeRepository _unidadeRepository;
    private readonly IColaboradorRepository _colaboradorRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IConfiguracaoMensalProdutoRepository _configuracaoRepository;

    public RetiradaMensalService(
        IRetiradaMensalRepository retiradaRepository,
        IUnidadeRepository unidadeRepository,
        IColaboradorRepository colaboradorRepository,
        IUsuarioRepository usuarioRepository,
        IProdutoRepository produtoRepository,
        IConfiguracaoMensalProdutoRepository configuracaoRepository)
    {
        _retiradaRepository = retiradaRepository;
        _unidadeRepository = unidadeRepository;
        _colaboradorRepository = colaboradorRepository;
        _usuarioRepository = usuarioRepository;
        _produtoRepository = produtoRepository;
        _configuracaoRepository = configuracaoRepository;
    }

    public async Task<IReadOnlyList<RetiradaMensalResponse>> ListarAsync(
        CancellationToken cancellationToken)
    {
        var retiradas = await _retiradaRepository.ListarAsync(
            cancellationToken);

        return retiradas
            .Select(MapearResponse)
            .ToList();
    }

    public async Task<RetiradaMensalResponse?> ObterPorIdAsync(
        string retiradaMensalId,
        CancellationToken cancellationToken)
    {
        retiradaMensalId = NormalizarId(retiradaMensalId);

        var retirada = await _retiradaRepository.ObterPorIdAsync(
            retiradaMensalId,
            rastrear: false,
            cancellationToken);

        return retirada is null
            ? null
            : MapearResponse(retirada);
    }

    public async Task<RetiradaMensalResponse> CriarAsync(
        CriarRetiradaMensalRequest request,
        CancellationToken cancellationToken)
    {
        var anoMes = NormalizarAnoMes(request.AnoMes);
        var colaboradorId = NormalizarId(request.ColaboradorId);
        var usuarioId = NormalizarId(request.UsuarioId);

        ValidarAnoMes(anoMes);
        ValidarProdutos(request.Produtos);

        var jaExisteRetiradaNoMes =
         await _retiradaRepository.ExisteRetiradaDoColaboradorNoMesAsync(
        anoMes,
        colaboradorId,
        cancellationToken);

        if (jaExisteRetiradaNoMes)
        {
            throw new ArgumentException(
                $"O colaborador {colaboradorId} já possui uma retirada no mês {anoMes}.");
        }

        var unidadeExiste = await _unidadeRepository.ExisteAsync(
            request.UnidadeId,
            cancellationToken);

        if (!unidadeExiste)
        {
            throw new ArgumentException(
                $"Unidade {request.UnidadeId} não encontrada.");
        }

        var colaborador = await _colaboradorRepository.ObterPorIdAsync(
            colaboradorId,
            rastrear: false,
            cancellationToken);

        if (colaborador is null)
        {
            throw new ArgumentException(
                $"Colaborador {colaboradorId} não encontrado.");
        }

        if (colaborador.UnidadeId != request.UnidadeId)
        {
            throw new ArgumentException(
                "O colaborador informado não pertence à unidade da retirada.");
        }

        if (colaborador.Tipo is null)
        {
            throw new ArgumentException(
                "O colaborador informado não possui tipo vinculado.");
        }

        var usuario = await _usuarioRepository.ObterPorIdAsync(
            usuarioId,
            rastrear: false,
            cancellationToken);

        if (usuario is null)
        {
            throw new ArgumentException(
                $"Usuário {usuarioId} não encontrado.");
        }

        if (usuario.UnidadeId != request.UnidadeId)
        {
            throw new ArgumentException(
                "O usuário informado não pertence à unidade da retirada.");
        }

        var produtosNormalizados = request.Produtos
            .Select(produto => new ProdutoRetiradaRequest
            {
                ProdutoId = NormalizarProdutoId(produto.ProdutoId),
                Quantidade = produto.Quantidade
            })
            .ToList();

        var quantidadeTotal = produtosNormalizados.Sum(
            produto => produto.Quantidade);

        if (quantidadeTotal > colaborador.Tipo.QuantidadeRetirada)
        {
            throw new ArgumentException(
                $"A quantidade total retirada ({quantidadeTotal}) excede o limite do tipo {colaborador.Tipo.Descricao} ({colaborador.Tipo.QuantidadeRetirada}).");
        }

        foreach (var produtoRequest in produtosNormalizados)
        {
            var produtoExiste = await _produtoRepository.ExisteAsync(
                produtoRequest.ProdutoId,
                cancellationToken);

            if (!produtoExiste)
            {
                throw new ArgumentException(
                    $"Produto {produtoRequest.ProdutoId} não encontrado.");
            }

            var configuracao = await _configuracaoRepository.ObterPorChaveAsync(
                anoMes,
                request.UnidadeId,
                produtoRequest.ProdutoId,
                cancellationToken);

            if (configuracao is null)
            {
                throw new ArgumentException(
                    $"Não existe configuração para o produto {produtoRequest.ProdutoId} no mês {anoMes} e unidade {request.UnidadeId}.");
            }

            if (produtoRequest.Quantidade < configuracao.Minimo)
            {
                throw new ArgumentException(
                    $"A quantidade do produto {produtoRequest.ProdutoId} não pode ser menor que o mínimo configurado ({configuracao.Minimo}).");
            }

            if (produtoRequest.Quantidade > configuracao.Maximo)
            {
                throw new ArgumentException(
                    $"A quantidade do produto {produtoRequest.ProdutoId} não pode ser maior que o máximo configurado ({configuracao.Maximo}).");
            }
        }

        var retiradaMensalId = Guid.NewGuid().ToString();

        var retirada = new RetiradasMensai
        {
            RetiradaMensalId = retiradaMensalId,
            AnoMes = anoMes,
            UnidadeId = request.UnidadeId,
            ColaboradorId = colaboradorId,
            UsuarioId = usuarioId,
            Operador = string.IsNullOrWhiteSpace(request.Operador)
                ? usuario.Nome
                : request.Operador.Trim(),
            DataHora = DateTime.Now,
            RetiradoPor = NormalizarTextoOpcional(request.RetiradoPor),
            RetiradasMensaisProdutos = produtosNormalizados
                .Select(produto => new RetiradasMensaisProduto
                {
                    RetiradaMensalProdutoId = Guid.NewGuid().ToString(),
                    RetiradaMensalId = retiradaMensalId,
                    ProdutoId = produto.ProdutoId,
                    Quantidade = produto.Quantidade
                })
                .ToList()
        };

        await _retiradaRepository.AdicionarAsync(
            retirada,
            cancellationToken);

        await _retiradaRepository.SalvarAlteracoesAsync(
            cancellationToken);

        var retiradaCriada = await _retiradaRepository.ObterPorIdAsync(
            retirada.RetiradaMensalId,
            rastrear: false,
            cancellationToken);

        return retiradaCriada is null
            ? MapearResponse(retirada)
            : MapearResponse(retiradaCriada);
    }

    public async Task<bool> ExcluirAsync(
        string retiradaMensalId,
        CancellationToken cancellationToken)
    {
        retiradaMensalId = NormalizarId(retiradaMensalId);

        var retirada = await _retiradaRepository.ObterPorIdAsync(
            retiradaMensalId,
            rastrear: true,
            cancellationToken);

        if (retirada is null)
        {
            return false;
        }

        _retiradaRepository.Remover(retirada);

        await _retiradaRepository.SalvarAlteracoesAsync(
            cancellationToken);

        return true;
    }

    private static RetiradaMensalResponse MapearResponse(
        RetiradasMensai retirada)
    {
        var produtos = retirada.RetiradasMensaisProdutos
            .Select(item => new ProdutoRetiradaResponse
            {
                RetiradaMensalProdutoId = item.RetiradaMensalProdutoId,
                ProdutoId = item.ProdutoId,
                ProdutoDescricao = item.Produto?.Descricao,
                Quantidade = item.Quantidade
            })
            .OrderBy(item => item.ProdutoDescricao)
            .ToList();

        return new RetiradaMensalResponse
        {
            RetiradaMensalId = retirada.RetiradaMensalId,
            AnoMes = retirada.AnoMes,
            UnidadeId = retirada.UnidadeId,
            UnidadeDescricao = retirada.Unidade?.Descricao,
            ColaboradorId = retirada.ColaboradorId,
            ColaboradorNome = retirada.Colaborador?.Nome,
            UsuarioId = retirada.UsuarioId,
            UsuarioNome = retirada.Usuario?.Nome,
            Operador = retirada.Operador,
            DataHora = retirada.DataHora,
            RetiradoPor = retirada.RetiradoPor,
            QuantidadeTotal = produtos.Sum(produto => produto.Quantidade),
            Produtos = produtos
        };
    }

    private static void ValidarProdutos(
        List<ProdutoRetiradaRequest> produtos)
    {
        if (produtos is null || produtos.Count == 0)
        {
            throw new ArgumentException(
                "Informe pelo menos um produto para a retirada.");
        }

        foreach (var produto in produtos)
        {
            if (string.IsNullOrWhiteSpace(produto.ProdutoId))
            {
                throw new ArgumentException(
                    "Todos os produtos precisam ter ProdutoId.");
            }

            if (produto.Quantidade <= 0)
            {
                throw new ArgumentException(
                    $"A quantidade do produto {produto.ProdutoId} deve ser maior que zero.");
            }
        }

        var produtosDuplicados = produtos
            .Select(produto => NormalizarProdutoId(produto.ProdutoId))
            .GroupBy(produtoId => produtoId)
            .Where(grupo => grupo.Count() > 1)
            .Select(grupo => grupo.Key)
            .ToList();

        if (produtosDuplicados.Count > 0)
        {
            throw new ArgumentException(
                $"Produto repetido na retirada: {string.Join(", ", produtosDuplicados)}.");
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

    private static string? NormalizarTextoOpcional(
        string? texto)
    {
        return string.IsNullOrWhiteSpace(texto)
            ? null
            : texto.Trim();
    }

    public async Task<IReadOnlyList<RetiradaMensalResponse>> ListarPorAnoMesAsync(
    string anoMes,
    CancellationToken cancellationToken)
    {
        anoMes = NormalizarAnoMes(anoMes);

        ValidarAnoMes(anoMes);

        var retiradas = await _retiradaRepository.ListarPorAnoMesAsync(
            anoMes,
            cancellationToken);

        return retiradas
            .Select(MapearResponse)
            .ToList();
    }

    public async Task<IReadOnlyList<ColaboradorNaoRetirouResponse>> ListarColaboradoresQueNaoRetiraramAsync(
        string anoMes,
        int unidadeId,
        CancellationToken cancellationToken)
    {
        anoMes = NormalizarAnoMes(anoMes);

        ValidarAnoMesFormato(anoMes);

        var colaboradores = await _retiradaRepository
            .BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                anoMes,
                unidadeId,
                cancellationToken);

        return colaboradores
            .Select(colaborador => new ColaboradorNaoRetirouResponse
            {
                ColaboradorId = colaborador.ColaboradorId,
                ColaboradorNome = colaborador.Nome,
                UnidadeId = colaborador.UnidadeId,
                UnidadeDescricao = colaborador.Unidade?.Descricao
            })
            .ToList();
    }

    private static void ValidarAnoMesFormato(string anoMes)
    {
        const string mensagemInvalida =
            "O campo anoMes deve estar no formato AAAAMM e conter um mês válido.";

        if (anoMes.Length != 6 || !anoMes.All(char.IsDigit))
        {
            throw new ArgumentException(mensagemInvalida);
        }

        if (!int.TryParse(anoMes[..4], out var ano) || ano < 1900 || ano > 2200)
        {
            throw new ArgumentException(mensagemInvalida);
        }

        if (!int.TryParse(anoMes[4..], out var mes) || mes < 1 || mes > 12)
        {
            throw new ArgumentException(mensagemInvalida);
        }
    }
}