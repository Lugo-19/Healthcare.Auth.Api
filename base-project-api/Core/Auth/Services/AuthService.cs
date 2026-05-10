using Healthcare.Auth.Api.Core.Auth.Dtos.Request;
using Healthcare.Auth.Api.Core.Auth.Dtos.Response;
using Healthcare.Auth.Api.Core.Auth.Interfaces;
using Healthcare.Auth.Api.Shared.Commons.Exceptions;
using Healthcare.Auth.Api.Shared.Helpers;

namespace Healthcare.Auth.Api.Core.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly JwtHelper _jwt;

        public AuthService(IAuthRepository repository, JwtHelper jwt)
        {
            _repository = repository;
            _jwt = jwt;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, string? ip, string? dispositivo)
        {
            var usuario = await _repository.ObtenerUsuarioPorCorreoAsync(request.Correo)
                ?? throw new UnauthorizedException("Credenciales inválidas.");

            if (usuario.IdEstado != 1)
                throw new UnauthorizedException("La cuenta está inactiva.");

            if (usuario.BloqueadoHasta.HasValue && usuario.BloqueadoHasta > DateTime.UtcNow)
                throw new UnauthorizedException($"Cuenta bloqueada temporalmente. Intente después de las {usuario.BloqueadoHasta:HH:mm}.");

            if (!BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.Contrasena))
            {
                await _repository.IncrementarIntentosFallidosAsync(request.Correo);
                throw new UnauthorizedException("Credenciales inválidas.");
            }

            var (accessToken, expiracion) = _jwt.GenerarAccessToken(usuario);
            var refreshToken = _jwt.GenerarRefreshToken();
            var fechaExpiracionRefresh = DateTime.UtcNow.AddDays(7);

            await _repository.GuardarRefreshTokenAsync(usuario.Id, refreshToken, fechaExpiracionRefresh, ip, dispositivo);
            await _repository.ActualizarUltimoAccesoAsync(usuario.Id);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiracion = expiracion,
                Usuario = new UsuarioInfo
                {
                    Id = usuario.Id,
                    NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
                    Correo = usuario.Correo,
                    Rol = usuario.NombreRol
                }
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ip, string? dispositivo)
        {
            var tokenData = await _repository.ObtenerRefreshTokenAsync(request.RefreshToken)
                ?? throw new UnauthorizedException("Refresh token inválido.");

            if (!tokenData.EsValido)
                throw new UnauthorizedException("El refresh token ha expirado o fue revocado.");

            if (tokenData.UsuarioIdEstado != 1)
                throw new UnauthorizedException("La cuenta está inactiva.");

            await _repository.RevocarRefreshTokenAsync(request.RefreshToken, tokenData.UsuarioCorreo);

            var usuarioTemp = new Entities.Usuario
            {
                Id = tokenData.IdUsuario,
                Nombre = tokenData.UsuarioNombre,
                Apellido = tokenData.UsuarioApellido,
                Correo = tokenData.UsuarioCorreo,
                IdRol = tokenData.UsuarioIdRol,
                NombreRol = tokenData.UsuarioNombreRol,
                IdEstado = tokenData.UsuarioIdEstado
            };

            var (accessToken, expiracion) = _jwt.GenerarAccessToken(usuarioTemp);
            var nuevoRefreshToken = _jwt.GenerarRefreshToken();

            await _repository.GuardarRefreshTokenAsync(usuarioTemp.Id, nuevoRefreshToken, DateTime.UtcNow.AddDays(7), ip, dispositivo);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = nuevoRefreshToken,
                Expiracion = expiracion,
                Usuario = new UsuarioInfo
                {
                    Id = usuarioTemp.Id,
                    NombreCompleto = $"{usuarioTemp.Nombre} {usuarioTemp.Apellido}",
                    Correo = usuarioTemp.Correo,
                    Rol = usuarioTemp.NombreRol
                }
            };
        }

        public async Task LogoutAsync(RefreshTokenRequest request, string correoUsuario)
        {
            var tokenData = await _repository.ObtenerRefreshTokenAsync(request.RefreshToken);
            if (tokenData != null && tokenData.EsValido)
                await _repository.RevocarRefreshTokenAsync(request.RefreshToken, correoUsuario);
        }
    }
}
