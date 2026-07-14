using Moq;
using PotyRetiradasApi.Dtos.RetiradasMensais;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;
using PotyRetiradasApi.Services;
using Xunit;

namespace PotyRetiradasApi.Tests.Services;

/// <summary>
/// Testes unitários da camada de serviço para o método
/// ListarColaboradoresQueNaoRetiraramAsync.
/// A filtragem real (Status, NOT EXISTS) é testada nos testes de repositório.
/// Aqui validamos a lógica de negócio, validação de parâmetros e mapeamento.
/// </summary>
public sealed class RetiradaMensalServiceColaboradoresNaoRetiraramTests
{
    private readonly Mock<IRetiradaMensalRepository> _retiradaRepoMock;
    private readonly RetiradaMensalService _service;

    public RetiradaMensalServiceColaboradoresNaoRetiraramTests()
    {
        _retiradaRepoMock = new Mock<IRetiradaMensalRepository>();

        _service = new RetiradaMensalService(
            _retiradaRepoMock.Object,
            new Mock<IUnidadeRepository>().Object,
            new Mock<IColaboradorRepository>().Object,
            new Mock<IUsuarioRepository>().Object,
            new Mock<IProdutoRepository>().Object,
            new Mock<IConfiguracaoMensalProdutoRepository>().Object);
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário 1 – Retorna colaborador sem retirada no período
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task RetornaColaboradorSemRetiradaNoPeriodo()
    {
        var unidade = new Unidade { UnidadeId = 1, Descricao = "Unidade Poty" };
        var colaboradores = new List<Colaboradore>
        {
            new()
            {
                ColaboradorId = "123",
                Nome = "João da Silva",
                UnidadeId = 1,
                Status = true,
                Unidade = unidade
            }
        };

        _retiradaRepoMock
            .Setup(r => r.BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(colaboradores);

        var resultado = await _service.ListarColaboradoresQueNaoRetiraramAsync(
            "202606", 1, CancellationToken.None);

        Assert.Single(resultado);
        Assert.Equal("123", resultado[0].ColaboradorId);
        Assert.Equal("João da Silva", resultado[0].ColaboradorNome);
        Assert.Equal(1, resultado[0].UnidadeId);
        Assert.Equal("Unidade Poty", resultado[0].UnidadeDescricao);
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário 8 – Retorna lista vazia quando todos retiraram
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task RetornaListaVaziaQuandoTodosRetiraram()
    {
        _retiradaRepoMock
            .Setup(r => r.BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Colaboradore>());

        var resultado = await _service.ListarColaboradoresQueNaoRetiraramAsync(
            "202606", 1, CancellationToken.None);

        Assert.Empty(resultado);
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário 9 – Não retorna duplicados (cada colaborador uma vez)
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task NaoRetornaDuplicados()
    {
        var unidade = new Unidade { UnidadeId = 1, Descricao = "Unidade Poty" };
        // O repositório já deve garantir unicidade via NOT EXISTS.
        // O serviço mapeia 1-para-1 sem introduzir duplicatas.
        var colaboradores = new List<Colaboradore>
        {
            new() { ColaboradorId = "A", Nome = "Ana", UnidadeId = 1, Status = true, Unidade = unidade },
            new() { ColaboradorId = "B", Nome = "Bruno", UnidadeId = 1, Status = true, Unidade = unidade }
        };

        _retiradaRepoMock
            .Setup(r => r.BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(colaboradores);

        var resultado = await _service.ListarColaboradoresQueNaoRetiraramAsync(
            "202606", 1, CancellationToken.None);

        Assert.Equal(2, resultado.Count);
        Assert.Equal(2, resultado.Select(c => c.ColaboradorId).Distinct().Count());
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário 13 – Ordena resultado pelo nome do colaborador
    // (o repositório garante a ordem; o serviço preserva a ordem do repositório)
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task PreservaOrdenacaoAlfabeticaDoRepositorio()
    {
        var unidade = new Unidade { UnidadeId = 1, Descricao = "Unidade Poty" };
        // O repositório já retorna os colaboradores ordenados por nome.
        var colaboradores = new List<Colaboradore>
        {
            new() { ColaboradorId = "A", Nome = "Ana", UnidadeId = 1, Status = true, Unidade = unidade },
            new() { ColaboradorId = "B", Nome = "Bruno", UnidadeId = 1, Status = true, Unidade = unidade },
            new() { ColaboradorId = "C", Nome = "Carlos", UnidadeId = 1, Status = true, Unidade = unidade }
        };

        _retiradaRepoMock
            .Setup(r => r.BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(colaboradores);

        var resultado = await _service.ListarColaboradoresQueNaoRetiraramAsync(
            "202606", 1, CancellationToken.None);

        Assert.Equal("Ana", resultado[0].ColaboradorNome);
        Assert.Equal("Bruno", resultado[1].ColaboradorNome);
        Assert.Equal("Carlos", resultado[2].ColaboradorNome);
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário 10 – Retorna erro para anoMes inválido
    // ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("2026")]       // apenas 4 dígitos
    [InlineData("202613")]     // mês 13 inválido
    [InlineData("202600")]     // mês 00 inválido
    [InlineData("ABCDEF")]     // não numérico
    [InlineData("2026061")]    // 7 dígitos
    [InlineData("20260")]      // 5 dígitos
    [InlineData("")]           // vazio
    [InlineData("   ")]        // somente espaços
    public async Task RetornaArgumentExceptionParaAnoMesInvalido(string anoMes)
    {
        var excecao = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.ListarColaboradoresQueNaoRetiraramAsync(anoMes, 1, CancellationToken.None));

        Assert.Equal(
            "O campo anoMes deve estar no formato AAAAMM e conter um mês válido.",
            excecao.Message);
    }

    [Theory]
    [InlineData("202601")]  // janeiro
    [InlineData("202606")]  // junho
    [InlineData("202612")]  // dezembro
    public async Task AceitaAnoMesValido(string anoMes)
    {
        _retiradaRepoMock
            .Setup(r => r.BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                anoMes, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Colaboradore>());

        // Não deve lançar exceção
        var resultado = await _service.ListarColaboradoresQueNaoRetiraramAsync(
            anoMes, 1, CancellationToken.None);

        Assert.Empty(resultado);
    }

    // ──────────────────────────────────────────────────────────────
    // Delegação correta ao repositório com os parâmetros normalizados
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task DelegaAoRepositorioComParametrosNormalizados()
    {
        _retiradaRepoMock
            .Setup(r => r.BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Colaboradore>());

        // anoMes com espaços extras deve ser normalizado antes da delegação
        await _service.ListarColaboradoresQueNaoRetiraramAsync(
            "  202606  ", 1, CancellationToken.None);

        _retiradaRepoMock.Verify(
            r => r.BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
