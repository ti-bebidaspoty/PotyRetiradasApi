using PotyRetiradasApi.Dtos.Unidades;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Services;

public sealed class UnidadeService : IUnidadeService
{
    private readonly IUnidadeRepository _repository;

    public UnidadeService(IUnidadeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<UnidadeResponse>> ListarAsync(
        CancellationToken cancellationToken)
    {
        var unidades = await _repository.ListarAsync(
            cancellationToken);

        return unidades
            .Select(MapearResponse)
            .ToList();
    }

    public async Task<UnidadeResponse?> ObterPorIdAsync(
        int unidadeId,
        CancellationToken cancellationToken)
    {
        var unidade = await _repository.ObterPorIdAsync(
            unidadeId,
            rastrear: false,
            cancellationToken);

        return unidade is null
            ? null
            : MapearResponse(unidade);
    }

    public async Task<UnidadeResponse?> CriarAsync(
        CriarUnidadeRequest request,
        CancellationToken cancellationToken)
    {
        var unidadeJaExiste = await _repository.ExisteAsync(
            request.UnidadeId,
            cancellationToken);

        if (unidadeJaExiste)
        {
            return null;
        }

        var unidade = new Unidade
        {
            UnidadeId = request.UnidadeId,
            Descricao = request.Descricao.Trim(),
            Status = request.Status
        };

        await _repository.AdicionarAsync(
            unidade,
            cancellationToken);

        await _repository.SalvarAlteracoesAsync(
            cancellationToken);

        return MapearResponse(unidade);
    }

    public async Task<UnidadeResponse?> AtualizarAsync(
        int unidadeId,
        AtualizarUnidadeRequest request,
        CancellationToken cancellationToken)
    {
        var unidade = await _repository.ObterPorIdAsync(
            unidadeId,
            rastrear: true,
            cancellationToken);

        if (unidade is null)
        {
            return null;
        }

        unidade.Descricao = request.Descricao.Trim();
        unidade.Status = request.Status;

        await _repository.SalvarAlteracoesAsync(
            cancellationToken);

        return MapearResponse(unidade);
    }

    public async Task<bool> ExcluirAsync(
        int unidadeId,
        CancellationToken cancellationToken)
    {
        var unidade = await _repository.ObterPorIdAsync(
            unidadeId,
            rastrear: true,
            cancellationToken);

        if (unidade is null)
        {
            return false;
        }

        _repository.Remover(unidade);

        await _repository.SalvarAlteracoesAsync(
            cancellationToken);

        return true;
    }

    private static UnidadeResponse MapearResponse(Unidade unidade)
    {
        return new UnidadeResponse
        {
            UnidadeId = unidade.UnidadeId,
            Descricao = unidade.Descricao,
            Status = unidade.Status
        };
    }
}