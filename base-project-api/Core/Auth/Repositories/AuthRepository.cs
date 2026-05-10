using Healthcare.Auth.Api.Core.Auth.Entities;
using Healthcare.Auth.Api.Core.Auth.Interfaces;
using Healthcare.Auth.Api.Shared.Helpers;

namespace Healthcare.Auth.Api.Core.Auth.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DbExecutor _db;

        public AuthRepository(DbExecutor db)
        {
            _db = db;
        }

        public async Task<Usuario?> ObtenerUsuarioPorCorreoAsync(string correo)
            => await _db.QuerySingleAsync<Usuario>("auth.sp_obtener_usuario_por_correo", new { p_correo = correo });

        public async Task GuardarRefreshTokenAsync(Guid idUsuario, string token, DateTime fechaExpiracion, string? ip, string? dispositivo)
            => await _db.ExecuteAsync("auth.sp_guardar_refresh_token", new
            {
                p_id_usuario = idUsuario,
                p_token = token,
                p_fecha_expiracion = fechaExpiracion,
                p_ip = ip,
                p_dispositivo = dispositivo
            });

        public async Task<RefreshToken?> ObtenerRefreshTokenAsync(string token)
            => await _db.QuerySingleAsync<RefreshToken>("auth.sp_obtener_refresh_token", new { p_token = token });

        public async Task RevocarRefreshTokenAsync(string token, string usuario)
            => await _db.ExecuteAsync("auth.sp_revocar_refresh_token", new { p_token = token, p_usuario = usuario });

        public async Task ActualizarUltimoAccesoAsync(Guid idUsuario)
            => await _db.ExecuteAsync("auth.sp_actualizar_ultimo_acceso", new { p_id_usuario = idUsuario });

        public async Task IncrementarIntentosFallidosAsync(string correo)
            => await _db.ExecuteAsync("auth.sp_incrementar_intentos_fallidos", new { p_correo = correo });
    }
}
