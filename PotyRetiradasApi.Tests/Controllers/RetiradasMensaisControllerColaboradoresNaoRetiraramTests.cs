using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PotyRetiradasApi.Controllers;
using PotyRetiradasApi.Dtos.RetiradasMensais;
using PotyRetiradasApi.Services.Interfaces;
using Xunit;

namespace PotyRetiradasApi.Tests.Controllers;

/// <summary>
/// Testes unitários do controller para o endpoint colaboradores-nao-retiraram.
/// Cobrem validação de parâmetros (cenário 11) e autorização (cenário 12).
/// </summary>
public sealed class RetiradasMensaisControllerColaboradoresNaoRetiraramTests
{
    private readonly Mock<IRetiradaMensalService> _serviceMock;
    private readonly RetiradasMensaisController _controller;

    public RetiradasMensaisControllerColaboradoresNaoRetiraramTests()
    {
        _serviceMock = new Mock<IRetiradaMensalService>();
        _controller = new RetiradasMensaisController(_serviceMock.Object);
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário 11 – unidadeId inválido retorna 400
    // ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task RetornaBadRequestParaUnidadeIdInvalido(int unidadeId)
    {
        ConfigurarUsuarioAutenticado(_controller, unidadeId: 1, administrador: false);

        var resultado = await _controller.ListarColaboradoresQueNaoRetiraram(
            "202606", unidadeId, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
        var corpo = badRequest.Value!.ToString();
        Assert.Contains("unidadeId", corpo);
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário 12 – Usuário não administrador não pode consultar outra unidade
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task NaoAdminNaoPodeConsultarOutraUnidade()
    {
        // Usuário pertence à unidade 1, mas tenta consultar unidade 2
        ConfigurarUsuarioAutenticado(_controller, unidadeId: 1, administrador: false);

        var resultado = await _controller.ListarColaboradoresQueNaoRetiraram(
            "202606", 2, CancellationToken.None);

        Assert.IsType<ForbidResult>(resultado.Result);
    }

    [Fact]
    public async Task NaoAdminPodeConsultarSuaPropriaUnidade()
    {
        ConfigurarUsuarioAutenticado(_controller, unidadeId: 1, administrador: false);

        _serviceMock
            .Setup(s => s.ListarColaboradoresQueNaoRetiraramAsync(
                "202606", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ColaboradorNaoRetirouResponse>());

        var resultado = await _controller.ListarColaboradoresQueNaoRetiraram(
            "202606", 1, CancellationToken.None);

        Assert.IsType<OkObjectResult>(resultado.Result);
    }

    [Fact]
    public async Task AdministradorPodeConsultarQualquerUnidade()
    {
        ConfigurarUsuarioAutenticado(_controller, unidadeId: 1, administrador: true);

        _serviceMock
            .Setup(s => s.ListarColaboradoresQueNaoRetiraramAsync(
                "202606", 99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ColaboradorNaoRetirouResponse>());

        var resultado = await _controller.ListarColaboradoresQueNaoRetiraram(
            "202606", 99, CancellationToken.None);

        Assert.IsType<OkObjectResult>(resultado.Result);
    }

    // ──────────────────────────────────────────────────────────────
    // Cenário 10 – anoMes inválido vindo do serviço retorna 400
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task RetornaBadRequestQuandoServicoLancaArgumentException()
    {
        ConfigurarUsuarioAutenticado(_controller, unidadeId: 1, administrador: false);

        _serviceMock
            .Setup(s => s.ListarColaboradoresQueNaoRetiraramAsync(
                It.IsAny<string>(), 1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException(
                "O campo anoMes deve estar no formato AAAAMM e conter um mês válido."));

        var resultado = await _controller.ListarColaboradoresQueNaoRetiraram(
            "202613", 1, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
        var corpo = badRequest.Value!.ToString();
        Assert.Contains("anoMes", corpo);
    }

    // ──────────────────────────────────────────────────────────────
    // Retorna 200 com lista vazia (não 404)
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task RetornaOkComListaVaziaQuandoTodosRetiraram()
    {
        ConfigurarUsuarioAutenticado(_controller, unidadeId: 1, administrador: false);

        _serviceMock
            .Setup(s => s.ListarColaboradoresQueNaoRetiraramAsync(
                "202606", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ColaboradorNaoRetirouResponse>());

        var resultado = await _controller.ListarColaboradoresQueNaoRetiraram(
            "202606", 1, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);
        var lista = Assert.IsAssignableFrom<IReadOnlyList<ColaboradorNaoRetirouResponse>>(ok.Value);
        Assert.Empty(lista);
    }

    // ──────────────────────────────────────────────────────────────
    // Helper – Configura o ClaimsPrincipal no controller
    // ──────────────────────────────────────────────────────────────

    private static void ConfigurarUsuarioAutenticado(
        ControllerBase controller,
        int unidadeId,
        bool administrador)
    {
        var claims = new List<Claim>
        {
            new("unidadeId", unidadeId.ToString()),
            new("administrador", administrador.ToString().ToLowerInvariant())
        };

        if (administrador)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Administrador"));
        }

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }
}
