using System.ComponentModel.DataAnnotations;

namespace PotyRetiradasApi.Dtos.RetiradasMensais;

public sealed class CriarRetiradaMensalRequest
{
    [Required(ErrorMessage = "O ano/mês é obrigatório.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "O ano/mês deve estar no formato yyyyMM.")]
    public string AnoMes { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "A unidade é obrigatória.")]
    public int UnidadeId { get; set; }

    [Required(ErrorMessage = "O colaborador é obrigatório.")]
    [StringLength(50, ErrorMessage = "O ID do colaborador deve possuir no máximo 50 caracteres.")]
    public string ColaboradorId { get; set; } = string.Empty;

    [Required(ErrorMessage = "O usuário é obrigatório.")]
    [StringLength(50, ErrorMessage = "O ID do usuário deve possuir no máximo 50 caracteres.")]
    public string UsuarioId { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "O operador deve possuir no máximo 100 caracteres.")]
    public string? Operador { get; set; }

    [StringLength(100, ErrorMessage = "O retirado por deve possuir no máximo 100 caracteres.")]
    public string? RetiradoPor { get; set; }

    [Required(ErrorMessage = "Informe pelo menos um produto.")]
    public List<ProdutoRetiradaRequest> Produtos { get; set; } = new();
}

public sealed class ColaboradorNaoRetirouResponse
{
    public string ColaboradorId { get; set; } = string.Empty;

    public string ColaboradorNome { get; set; } = string.Empty;

    public int UnidadeId { get; set; }

    public string? UnidadeDescricao { get; set; }
}

public sealed class RetiradaMensalResponse
{
    public string RetiradaMensalId { get; set; } = string.Empty;

    public string AnoMes { get; set; } = string.Empty;

    public int UnidadeId { get; set; }

    public string? UnidadeDescricao { get; set; }

    public string ColaboradorId { get; set; } = string.Empty;

    public string? ColaboradorNome { get; set; }

    public string UsuarioId { get; set; } = string.Empty;

    public string? UsuarioNome { get; set; }

    public string Operador { get; set; } = string.Empty;

    public DateTime DataHora { get; set; }

    public string? RetiradoPor { get; set; }

    public int QuantidadeTotal { get; set; }

    public List<ProdutoRetiradaResponse> Produtos { get; set; } = new();
}