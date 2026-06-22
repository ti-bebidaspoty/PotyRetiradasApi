using System.ComponentModel.DataAnnotations;

namespace PotyRetiradasApi.Dtos.Auth;

public sealed class LoginRequest
{
    [Required(ErrorMessage = "O usuário é obrigatório.")]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; } = string.Empty;
}

public sealed class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiraEm { get; set; }

    public UsuarioAutenticadoResponse Usuario { get; set; } = new();
}

public sealed class UsuarioAutenticadoResponse
{
    public string UsuarioId { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string Login { get; set; } = string.Empty;

    public bool Administrador { get; set; }

    public int UnidadeId { get; set; }
}