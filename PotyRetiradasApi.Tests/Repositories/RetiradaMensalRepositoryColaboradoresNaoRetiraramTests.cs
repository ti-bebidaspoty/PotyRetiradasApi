using Microsoft.EntityFrameworkCore;
using PotyRetiradasApi.Data;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories;
using Xunit;

namespace PotyRetiradasApi.Tests.Repositories;

/// <summary>
/// Testes de integração do repositório usando banco de dados in-memory.
/// Validam a lógica de filtragem SQL (Status, NOT EXISTS, ordenação, unicidade).
/// </summary>
public sealed class RetiradaMensalRepositoryColaboradoresNaoRetiraramTests : IDisposable
{
    private readonly RetiradasDbContext _context;
    private readonly RetiradaMensalRepository _repository;

    // Dados de suporte reutilizados entre testes
    private readonly Unidade _unidade1 = new() { UnidadeId = 1, Descricao = "Unidade Poty", Status = true };
    private readonly Unidade _unidade2 = new() { UnidadeId = 2, Descricao = "Outra Unidade", Status = true };

    public RetiradaMensalRepositoryColaboradoresNaoRetiraramTests()
    {
        var options = new DbContextOptionsBuilder<RetiradasDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new RetiradasDbContext(options);
        _repository = new RetiradaMensalRepository(_context);

        _context.Unidades.AddRange(_unidade1, _unidade2);
        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();

    // ──────────────────────────────────────────────────────────────
    // Cenário 1 – Colaborador sem nenhuma retirada é retornado
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task RetornaColaboradorSemNenhumaRetirada()
    {
        var colaborador = CriarColaborador("C001", "Ana", 1, ativo: true);
        _context.Colaboradores.Add(colaborador);
        await _context.SaveChangesAsync();

        var resultado = await _repository
            .BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, CancellationToken.None);

        Assert.Single(resultado);
        Assert.Equal("C001", resultado[0].ColaboradorId);
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário 2 – Colaborador com retirada no mesmo mês NÃO é retornado
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task NaoRetornaColaboradorComRetiradaNoMesmo_Mes()
    {
        var colaborador = CriarColaborador("C001", "Ana", 1, ativo: true);
        _context.Colaboradores.Add(colaborador);

        _context.RetiradasMensais.Add(CriarRetirada("R001", "202606", 1, "C001"));
        await _context.SaveChangesAsync();

        var resultado = await _repository
            .BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, CancellationToken.None);

        Assert.Empty(resultado);
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário 3 – Retirada em outro mês não exclui o colaborador
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task RetornaColaboradorComRetiradaEmOutroMes()
    {
        var colaborador = CriarColaborador("C001", "Ana", 1, ativo: true);
        _context.Colaboradores.Add(colaborador);

        // Possui retirada em maio (202605), consulta é para junho (202606)
        _context.RetiradasMensais.Add(CriarRetirada("R001", "202605", 1, "C001"));
        await _context.SaveChangesAsync();

        var resultado = await _repository
            .BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, CancellationToken.None);

        Assert.Single(resultado);
        Assert.Equal("C001", resultado[0].ColaboradorId);
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário 4 – Retirada em outra unidade não exclui o colaborador
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task RetornaColaboradorComRetiradaEmOutraUnidade()
    {
        var colaborador = CriarColaborador("C001", "Ana", 1, ativo: true);
        _context.Colaboradores.Add(colaborador);

        // Retirada para unidade 2, mas colaborador pertence à unidade 1
        _context.RetiradasMensais.Add(CriarRetirada("R001", "202606", 2, "C001"));
        await _context.SaveChangesAsync();

        var resultado = await _repository
            .BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, CancellationToken.None);

        Assert.Single(resultado);
        Assert.Equal("C001", resultado[0].ColaboradorId);
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário 5 – Colaborador inativo não é retornado
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task NaoRetornaColaboradorInativo()
    {
        var colaborador = CriarColaborador("C001", "Ana", 1, ativo: false);
        _context.Colaboradores.Add(colaborador);
        await _context.SaveChangesAsync();

        var resultado = await _repository
            .BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, CancellationToken.None);

        Assert.Empty(resultado);
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário 9 – Sem duplicados (mesmo colaborador, múltiplas retiradas em outros meses)
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task NaoRetornaDuplicados()
    {
        var colaborador = CriarColaborador("C001", "Ana", 1, ativo: true);
        _context.Colaboradores.Add(colaborador);

        // Retiradas em outros meses — não devem excluí-lo nem duplicá-lo
        _context.RetiradasMensais.AddRange(
            CriarRetirada("R001", "202604", 1, "C001"),
            CriarRetirada("R002", "202605", 1, "C001"));
        await _context.SaveChangesAsync();

        var resultado = await _repository
            .BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, CancellationToken.None);

        Assert.Single(resultado);
        Assert.Equal("C001", resultado[0].ColaboradorId);
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário 13 – Resultado ordenado alfabeticamente pelo nome
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task RetornaColaboradoresOrdenadosPeloNome()
    {
        _context.Colaboradores.AddRange(
            CriarColaborador("C003", "Carlos", 1, ativo: true),
            CriarColaborador("C001", "Ana", 1, ativo: true),
            CriarColaborador("C002", "Bruno", 1, ativo: true));
        await _context.SaveChangesAsync();

        var resultado = await _repository
            .BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, CancellationToken.None);

        Assert.Equal(3, resultado.Count);
        Assert.Equal("Ana", resultado[0].Nome);
        Assert.Equal("Bruno", resultado[1].Nome);
        Assert.Equal("Carlos", resultado[2].Nome);
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário misto – Somente retorna colaboradores SEM retirada no período
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task RetornaApenasColaboradoresSemRetiradaNoPeriodo()
    {
        var semRetirada = CriarColaborador("C001", "Ana", 1, ativo: true);
        var comRetirada = CriarColaborador("C002", "Bruno", 1, ativo: true);
        _context.Colaboradores.AddRange(semRetirada, comRetirada);

        _context.RetiradasMensais.Add(CriarRetirada("R001", "202606", 1, "C002"));
        await _context.SaveChangesAsync();

        var resultado = await _repository
            .BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, CancellationToken.None);

        Assert.Single(resultado);
        Assert.Equal("C001", resultado[0].ColaboradorId);
    }

    // ──────────────────────────────────────────────────────────────
    // Somente colaboradores da unidade consultada são retornados
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task RetornaApenasColaboradoresDaUnidadeConsultada()
    {
        _context.Colaboradores.AddRange(
            CriarColaborador("C001", "Ana", 1, ativo: true),
            CriarColaborador("C002", "Bruno", 2, ativo: true));
        await _context.SaveChangesAsync();

        var resultado = await _repository
            .BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
                "202606", 1, CancellationToken.None);

        Assert.Single(resultado);
        Assert.Equal("C001", resultado[0].ColaboradorId);
    }

    // ──────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────

    private static Colaboradore CriarColaborador(
        string id, string nome, int unidadeId, bool ativo)
        => new()
        {
            ColaboradorId = id,
            Nome = nome,
            UnidadeId = unidadeId,
            Status = ativo
        };

    private static RetiradasMensai CriarRetirada(
        string id, string anoMes, int unidadeId, string colaboradorId)
        => new()
        {
            RetiradaMensalId = id,
            AnoMes = anoMes,
            UnidadeId = unidadeId,
            ColaboradorId = colaboradorId,
            UsuarioId = "U001",
            Operador = "Sistema",
            DataHora = DateTime.Now
        };
}
