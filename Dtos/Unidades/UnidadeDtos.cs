using System.ComponentModel.DataAnnotations;

namespace PotyRetiradasApi.Dtos.Unidades;

public sealed class CriarUnidadeRequest
{
    [Range(1, int.MaxValue)]
    public int UnidadeId { get; set; }

    [Required]
    [StringLength(100)]
    public string Descricao { get; set; } = string.Empty;

    public bool Status { get; set; } = true;
}

public sealed class AtualizarUnidadeRequest
{
    [Required]
    [StringLength(100)]
    public string Descricao { get; set; } = string.Empty;

    public bool Status { get; set; }
}

public sealed class UnidadeResponse
{
    public int UnidadeId { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public bool Status { get; set; }
}