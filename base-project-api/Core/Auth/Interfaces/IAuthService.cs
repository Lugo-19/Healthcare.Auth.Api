using Healthcare.Auth.Api.Core.Auth.Dtos.Request;
using Healthcare.Auth.Api.Core.Auth.Dtos.Response;

namespace Healthcare.Auth.Api.Core.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request, string? ip, string? dispositivo);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ip, string? dispositivo);
        Task LogoutAsync(RefreshTokenRequest request, string correoUsuario);
    }
}
