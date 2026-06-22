using System.ComponentModel.DataAnnotations;

namespace PotyRetiradasApi.Dtos.Usuarios;

public sealed class CriarUsuarioRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve possuir no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O usuário é obrigatório.")]
    [StringLength(100, ErrorMessage = "O usuário deve possuir no máximo 100 caracteres.")]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve possuir entre 6 e 100 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    public bool Administrador { get; set; }

    public bool Status { get; set; } = true;

    [Range(1, int.MaxValue, ErrorMessage = "A unidade é obrigatória.")]
    public int UnidadeId { get; set; }
}

public sealed class AtualizarUsuarioRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve possuir no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O usuário é obrigatório.")]
    [StringLength(100, ErrorMessage = "O usuário deve possuir no máximo 100 caracteres.")]
    public string Usuario { get; set; } = string.Empty;

    // Se vier vazio ou null, não altera a senha.
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve possuir entre 6 e 100 caracteres.")]
    public string? Senha { get; set; }

    public bool Administrador { get; set; }

    public bool Status { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A unidade é obrigatória.")]
    public int UnidadeId { get; set; }
}

public sealed class UsuarioResponse
{
    public string UsuarioId { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string Usuario { get; set; } = string.Empty;

    public bool Administrador { get; set; }

    public bool Status { get; set; }

    public int UnidadeId { get; set; }
}