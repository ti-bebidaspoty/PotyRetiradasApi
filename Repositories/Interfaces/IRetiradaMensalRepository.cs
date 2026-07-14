using PotyRetiradasApi.Entities;

namespace PotyRetiradasApi.Repositories.Interfaces;

public interface IRetiradaMensalRepository
{
    Task<IReadOnlyList<RetiradasMensai>> ListarAsync(
        CancellationToken cancellationToken);

    Task<RetiradasMensai?> ObterPorIdAsync(
        string retiradaMensalId,
        bool rastrear,
        CancellationToken cancellationToken);

    Task AdicionarAsync(
        RetiradasMensai retirada,
        CancellationToken cancellationToken);

    void Remover(
        RetiradasMensai retirada);

    Task<int> SalvarAlteracoesAsync(
        CancellationToken cancellationToken);

    Task<bool> ExisteRetiradaDoColaboradorNoMesAsync(
    string anoMes,
    string colaboradorId,
    CancellationToken cancellationToken);

    Task<IReadOnlyList<RetiradasMensai>> ListarPorAnoMesAsync(
    string anoMes,
    CancellationToken cancellationToken);

    /// <summary>
    /// Retorna colaboradores ativos da unidade que não possuêm nenhuma retirada registrada no período.
    /// A consulta utiliza NOT EXISTS e não carrega todas as retiradas em memória.
    /// NOTA: Como a entidade Produto não possui campo de classificação de refrigerante,
    /// e todo o sistema é dedicado à retirada de refrigerantes, a existência de qualquer
    /// registro de RetiradasMensai para o período já confirma que o colaborador retirou.
    /// </summary>
    Task<IReadOnlyList<Colaboradore>> BuscarColaboradoresSemRetiradaPorAnoMesEUnidadeAsync(
        string anoMes,
        int unidadeId,
        CancellationToken cancellationToken);
}