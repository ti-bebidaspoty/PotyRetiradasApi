using System.ComponentModel.DataAnnotations;

namespace PotyRetiradasApi.Dtos.Tipos;

public sealed class CriarTipoRequest
{
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(100, ErrorMessage = "A descrição deve possuir no máximo 100 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "A quantidade de retirada não pode ser negativa.")]
    public int QuantidadeRetirada { get; set; }
}

public sealed class AtualizarTipoRequest
{
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(100, ErrorMessage = "A descrição deve possuir no máximo 100 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "A quantidade de retirada não pode ser negativa.")]
    public int QuantidadeRetirada { get; set; }
}

public sealed class TipoResponse
{
    public string TipoId { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public int QuantidadeRetirada { get; set; }
}