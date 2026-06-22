using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PotyRetiradasApi.Configurations;
using PotyRetiradasApi.Dtos.Auth;
using PotyRetiradasApi.Entities;
using PotyRetiradasApi.Repositories.Interfaces;
using PotyRetiradasApi.Services.Interfaces;

namespace PotyRetiradasApi.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher<Usuario> passwordHasher,
        IOptions<JwtOptions> jwtOptions)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginResponse?> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var login = NormalizarLogin(request.Usuario);

        var usuario = await _usuarioRepository.ObterPorLoginAsync(
            login,
            cancellationToken);

        if (usuario is null || !usuario.Status)
        {
            return null;
        }

        var resultadoSenha = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            request.Senha);

        if (resultadoSenha == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var expiraEm = DateTime.UtcNow.AddMinutes(
            _jwtOptions.ExpirationMinutes);

        var token = GerarToken(
            usuario,
            expiraEm);

        return new LoginResponse
        {
            Token = token,
            ExpiraEm = expiraEm,
            Usuario = new UsuarioAutenticadoResponse
            {
                UsuarioId = usuario.UsuarioId,
                Nome = usuario.Nome,
                Login = usuario.Usuario1,
                Administrador = usuario.Administrador,
                UnidadeId = usuario.UnidadeId
            }
        };
    }

    private string GerarToken(
        Usuario usuario,
        DateTime expiraEm)
    {
        if (string.IsNullOrWhiteSpace(_jwtOptions.Key))
        {
            throw new InvalidOperationException(
                "A chave JWT não foi configurada.");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.UsuarioId),
            new(JwtRegisteredClaimNames.UniqueName, usuario.Usuario1),
            new(ClaimTypes.NameIdentifier, usuario.UsuarioId),
            new(ClaimTypes.Name, usuario.Nome),
            new("usuarioId", usuario.UsuarioId),
            new("login", usuario.Usuario1),
            new("unidadeId", usuario.UnidadeId.ToString()),
            new("administrador", usuario.Administrador.ToString().ToLowerInvariant())
        };

        if (usuario.Administrador)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Administrador"));
        }

        var chave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.Key));

        var credenciais = new SigningCredentials(
            chave,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    private static string NormalizarLogin(
        string login)
    {
        return login.Trim().ToLowerInvariant();
    }
}