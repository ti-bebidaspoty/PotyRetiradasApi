using PotyRetiradasApi.Dtos.Colaboradores;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Services;

public sealed class ColaboradorService : IColaboradorService
{
    private readonly IColaboradorRepository _colaboradorRepository;
    private readonly IUnidadeRepository _unidadeRepository;
    private readonly ITipoRepository _tipoRepository;

    public ColaboradorService(
        IColaboradorRepository colaboradorRepository,
        IUnidadeRepository unidadeRepository,
        ITipoRepository tipoRepository)
    {
        _colaboradorRepository = colaboradorRepository;
        _unidadeRepository = unidadeRepository;
        _tipoRepository = tipoRepository;
    }

    public async Task<IReadOnlyList<ColaboradorResponse>> ListarAsync(
        CancellationToken cancellationToken)
    {
        var colaboradores = await _colaboradorRepository.ListarAsync(
            cancellationToken);

        return colaboradores
            .Select(MapearResponse)
            .ToList();
    }

    public async Task<ColaboradorResponse?> ObterPorIdAsync(
        string colaboradorId,
        CancellationToken cancellationToken)
    {
        colaboradorId = NormalizarColaboradorId(colaboradorId);

        var colaborador = await _colaboradorRepository.ObterPorIdAsync(
            colaboradorId,
            rastrear: false,
            cancellationToken);

        return colaborador is null
            ? null
            : MapearResponse(colaborador);
    }

    public async Task<ColaboradorResponse> CriarAsync(
    CriarColaboradorRequest request,
    CancellationToken cancellationToken)
    {
        var unidadeExiste = await _unidadeRepository.ExisteAsync(
            request.UnidadeId,
            cancellationToken);

        if (!unidadeExiste)
        {
            throw new ArgumentException(
                $"Unidade {request.UnidadeId} não encontrada.");
        }

        var tipoId = NormalizarTipoId(request.TipoId);

        var tipoExiste = await _tipoRepository.ExisteAsync(
            tipoId,
            cancellationToken);

        if (!tipoExiste)
        {
            throw new ArgumentException(
                $"Tipo {tipoId} não encontrado.");
        }

        var colaborador = new Colaboradore
        {
            ColaboradorId = Guid.NewGuid().ToString(),
            CodigoAlternativo = request.CodigoAlternativo,
            Nome = request.Nome.Trim(),
            UnidadeId = request.UnidadeId,
            TipoId = tipoId,
            Status = request.Status
        };

        await _colaboradorRepository.AdicionarAsync(
            colaborador,
            cancellationToken);

        await _colaboradorRepository.SalvarAlteracoesAsync(
            cancellationToken);

        return MapearResponse(colaborador);
    }

    public async Task<ColaboradorResponse?> AtualizarAsync(
    string colaboradorId,
    AtualizarColaboradorRequest request,
    CancellationToken cancellationToken)
    {
        colaboradorId = NormalizarColaboradorId(colaboradorId);

        var colaborador = await _colaboradorRepository.ObterPorIdAsync(
            colaboradorId,
            rastrear: true,
            cancellationToken);

        if (colaborador is null)
        {
            return null;
        }

        var unidadeExiste = await _unidadeRepository.ExisteAsync(
            request.UnidadeId,
            cancellationToken);

        if (!unidadeExiste)
        {
            throw new ArgumentException(
                $"Unidade {request.UnidadeId} não encontrada.");
        }

        var tipoId = NormalizarTipoId(request.TipoId);

        var tipoExiste = await _tipoRepository.ExisteAsync(
            tipoId,
            cancellationToken);

        if (!tipoExiste)
        {
            throw new ArgumentException(
                $"Tipo {tipoId} não encontrado.");
        }

        colaborador.CodigoAlternativo = request.CodigoAlternativo;
        colaborador.Nome = request.Nome.Trim();
        colaborador.UnidadeId = request.UnidadeId;
        colaborador.TipoId = tipoId;
        colaborador.Status = request.Status;

        await _colaboradorRepository.SalvarAlteracoesAsync(
            cancellationToken);

        return MapearResponse(colaborador);
    }

    public async Task<bool> DesativarAsync(
        string colaboradorId,
        CancellationToken cancellationToken)
    {
        colaboradorId = NormalizarColaboradorId(colaboradorId);

        var colaborador = await _colaboradorRepository.ObterPorIdAsync(
            colaboradorId,
            rastrear: true,
            cancellationToken);

        if (colaborador is null)
        {
            return false;
        }

        colaborador.Status = false;

        await _colaboradorRepository.SalvarAlteracoesAsync(
            cancellationToken);

        return true;
    }

    private static ColaboradorResponse MapearResponse(
    Colaboradore colaborador)
    {
        return new ColaboradorResponse
        {
            ColaboradorId = colaborador.ColaboradorId,
            CodigoAlternativo = colaborador.CodigoAlternativo,
            Nome = colaborador.Nome,
            UnidadeId = colaborador.UnidadeId,
            TipoId = colaborador.TipoId,
            TipoDescricao = colaborador.Tipo?.Descricao,
            Status = colaborador.Status
        };
    }

    private static string NormalizarTipoId(string tipoId)
    {
        return tipoId.Trim().ToUpperInvariant();
    }

    private static string NormalizarColaboradorId(
        string colaboradorId)
    {
        return colaboradorId.Trim();
    }
}