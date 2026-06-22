using System.ComponentModel.DataAnnotations;

namespace PotyRetiradasApi.Dtos.RetiradasMensais;

public sealed class ProdutoRetiradaRequest
{
    [Required(ErrorMessage = "O produto é obrigatório.")]
    [StringLength(8, ErrorMessage = "O ID do produto deve possuir no máximo 8 caracteres.")]
    public string ProdutoId { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantidade { get; set; }
}

public sealed class ProdutoRetiradaResponse
{
    public string RetiradaMensalProdutoId { get; set; } = string.Empty;

    public string ProdutoId { get; set; } = string.Empty;

    public string? ProdutoDescricao { get; set; }

    public int Quantidade { get; set; }
}