using System.ComponentModel.DataAnnotations;

namespace PotyRetiradasApi.Dtos.ConfiguracoesMensaisProdutos;

public sealed class CriarConfiguracaoMensalProdutoRequest
{
    [Required(ErrorMessage = "O ano/mês é obrigatório.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "O ano/mês deve estar no formato yyyyMM.")]
    public string AnoMes { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "A unidade é obrigatória.")]
    public int UnidadeId { get; set; }

    [Required(ErrorMessage = "O produto é obrigatório.")]
    [StringLength(8, ErrorMessage = "O ID do produto deve possuir no máximo 8 caracteres.")]
    public string ProdutoId { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "O mínimo não pode ser negativo.")]
    public int Minimo { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "O máximo não pode ser negativo.")]
    public int Maximo { get; set; }
}

public sealed class AtualizarConfiguracaoMensalProdutoRequest
{
    [Required(ErrorMessage = "O ano/mês é obrigatório.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "O ano/mês deve estar no formato yyyyMM.")]
    public string AnoMes { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "A unidade é obrigatória.")]
    public int UnidadeId { get; set; }

    [Required(ErrorMessage = "O produto é obrigatório.")]
    [StringLength(8, ErrorMessage = "O ID do produto deve possuir no máximo 8 caracteres.")]
    public string ProdutoId { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "O mínimo não pode ser negativo.")]
    public int Minimo { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "O máximo não pode ser negativo.")]
    public int Maximo { get; set; }
}

public sealed class ConfiguracaoMensalProdutoResponse
{
    public string ConfiguracaoMensalId { get; set; } = string.Empty;

    public string AnoMes { get; set; } = string.Empty;

    public int UnidadeId { get; set; }

    public string? UnidadeDescricao { get; set; }

    public string ProdutoId { get; set; } = string.Empty;

    public string? ProdutoDescricao { get; set; }

    public int Minimo { get; set; }

    public int Maximo { get; set; }
}

public sealed class ProdutoConfiguradoMesResponse
{
    public string ConfiguracaoMensalId { get; set; } = string.Empty;

    public string ProdutoId { get; set; } = string.Empty;

    public string ProdutoDescricao { get; set; } = string.Empty;

    public string? CodigoBarras { get; set; }

    public string? Imagem { get; set; }

    public int Minimo { get; set; }

    public int Maximo { get; set; }
}