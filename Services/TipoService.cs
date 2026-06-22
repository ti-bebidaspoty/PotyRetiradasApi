using PotyRetiradasApi.Dtos.Tipos;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Services;

public sealed class TipoService : ITipoService
{
    private readonly ITipoRepository _repository;

    public TipoService(ITipoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TipoResponse>> ListarAsync(
        CancellationToken cancellationToken)
    {
        var tipos = await _repository.ListarAsync(cancellationToken);

        return tipos
            .Select(MapearResponse)
            .ToList();
    }

    public async Task<TipoResponse?> ObterPorIdAsync(
        string tipoId,
        CancellationToken cancellationToken)
    {
        tipoId = NormalizarTipoId(tipoId);

        var tipo = await _repository.ObterPorIdAsync(
            tipoId,
            rastrear: false,
            cancellationToken);

        return tipo is null
            ? null
            : MapearResponse(tipo);
    }

    public async Task<TipoResponse> CriarAsync(
    CriarTipoRequest request,
    CancellationToken cancellationToken)
    {
        var tipo = new Tipo
        {
            TipoId = Guid.NewGuid().ToString(),
            Descricao = request.Descricao.Trim(),
            QuantidadeRetirada = request.QuantidadeRetirada
        };

        await _repository.AdicionarAsync(
            tipo,
            cancellationToken);

        await _repository.SalvarAlteracoesAsync(
            cancellationToken);

        return MapearResponse(tipo);
    }

    public async Task<TipoResponse?> AtualizarAsync(
        string tipoId,
        AtualizarTipoRequest request,
        CancellationToken cancellationToken)
    {
        tipoId = NormalizarTipoId(tipoId);

        var tipo = await _repository.ObterPorIdAsync(
            tipoId,
            rastrear: true,
            cancellationToken);

        if (tipo is null)
        {
            return null;
        }

        tipo.Descricao = request.Descricao.Trim();
        tipo.QuantidadeRetirada = request.QuantidadeRetirada;

        await _repository.SalvarAlteracoesAsync(cancellationToken);

        return MapearResponse(tipo);
    }

    public async Task<bool> ExcluirAsync(
    string tipoId,
    CancellationToken cancellationToken)
    {
        tipoId = NormalizarTipoId(tipoId);

        var tipo = await _repository.ObterPorIdAsync(
            tipoId,
            rastrear: true,
            cancellationToken);

        if (tipo is null)
        {
            return false;
        }

        _repository.Remover(tipo);

        await _repository.SalvarAlteracoesAsync(
            cancellationToken);

        return true;
    }

    private static TipoResponse MapearResponse(Tipo tipo)
    {
        return new TipoResponse
        {
            TipoId = tipo.TipoId,
            Descricao = tipo.Descricao,
            QuantidadeRetirada = tipo.QuantidadeRetirada
        };
    }

    private static string NormalizarTipoId(string tipoId)
    {
        return tipoId.Trim().ToUpperInvariant();
    }
}