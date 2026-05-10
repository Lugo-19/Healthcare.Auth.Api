using Healthcare.Auth.Api.Core.Auth.Entities;

namespace Healthcare.Auth.Api.Core.Auth.Interfaces
{
    public interface IAuthRepository
    {
        Task<Usuario?> ObtenerUsuarioPorCorreoAsync(string correo);
        Task GuardarRefreshTokenAsync(Guid idUsuario, string token, DateTime fechaExpiracion, string? ip, string? dispositivo);
        Task<RefreshToken?> ObtenerRefreshTokenAsync(string token);
        Task RevocarRefreshTokenAsync(string token, string usuario);
        Task ActualizarUltimoAccesoAsync(Guid idUsuario);
        Task IncrementarIntentosFallidosAsync(string correo);
    }
}
