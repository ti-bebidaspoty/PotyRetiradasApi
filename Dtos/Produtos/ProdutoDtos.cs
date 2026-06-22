using System.ComponentModel.DataAnnotations;

namespace PotyRetiradasApi.Dtos.Produtos;

public sealed class CriarProdutoRequest
{
    [Required(ErrorMessage = "O código do produto é obrigatório.")]
    [StringLength(8, ErrorMessage = "O código do produto deve possuir no máximo 8 caracteres.")]
    public string ProdutoId { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(200, ErrorMessage = "A descrição deve possuir no máximo 200 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "O código de barras deve possuir no máximo 50 caracteres.")]
    public string? CodigoBarras { get; set; }

    public string? Imagem { get; set; }

    public bool Status { get; set; } = true;
}

public sealed class AtualizarProdutoRequest
{
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(200, ErrorMessage = "A descrição deve possuir no máximo 200 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "O código de barras deve possuir no máximo 50 caracteres.")]
    public string? CodigoBarras { get; set; }

    public string? Imagem { get; set; }

    public bool Status { get; set; }
}

public sealed class ProdutoResponse
{
    public string ProdutoId { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public bool Status { get; set; }

    public string? CodigoBarras { get; set; }

    public string? Imagem { get; set; }
}