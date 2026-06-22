using PotyRetiradasApi.Dtos.Auth;

namespace PotyRetiradasApi.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);
}