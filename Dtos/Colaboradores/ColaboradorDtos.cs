using System.ComponentModel.DataAnnotations;

namespace PotyRetiradasApi.Dtos.Colaboradores;

public sealed class CriarColaboradorRequest
{
    public int? CodigoAlternativo { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, ErrorMessage = "O nome deve possuir no máximo 200 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "A unidade é obrigatória.")]
    public int UnidadeId { get; set; }

    [Required(ErrorMessage = "O tipo é obrigatório.")]
    [StringLength(50, ErrorMessage = "O tipo deve possuir no máximo 50 caracteres.")]
    public string TipoId { get; set; } = string.Empty;

    public bool Status { get; set; } = true;
}

public sealed class AtualizarColaboradorRequest
{
    public int? CodigoAlternativo { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, ErrorMessage = "O nome deve possuir no máximo 200 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "A unidade é obrigatória.")]
    public int UnidadeId { get; set; }

    [Required(ErrorMessage = "O tipo é obrigatório.")]
    [StringLength(50, ErrorMessage = "O tipo deve possuir no máximo 50 caracteres.")]
    public string TipoId { get; set; } = string.Empty;

    public bool Status { get; set; }
}

public sealed class ColaboradorResponse
{
    public string ColaboradorId { get; set; } = string.Empty;

    public int? CodigoAlternativo { get; set; }

    public string Nome { get; set; } = string.Empty;

    public int UnidadeId { get; set; }

    public string TipoId { get; set; } = string.Empty;

    public string? TipoDescricao { get; set; }

    public bool Status { get; set; }
}