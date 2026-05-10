using Asp.Versioning;
using Healthcare.Auth.Api.Core.Auth.Dtos.Request;
using Healthcare.Auth.Api.Core.Auth.Interfaces;
using Healthcare.Auth.Api.Shared.Commons;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Healthcare.Auth.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly IValidator<RefreshTokenRequest> _refreshValidator;

        public AuthController(IAuthService authService, IValidator<LoginRequest> loginValidator, IValidator<RefreshTokenRequest> refreshValidator)
        {
            _authService = authService;
            _loginValidator = loginValidator;
            _refreshValidator = refreshValidator;
        }

        [HttpPost("login")]
        public async Task<ApiResponse<object>> Login([FromBody] LoginRequest request)
        {
            var validacion = await _loginValidator.ValidateAsync(request);
            if (!validacion.IsValid)
                return ApiResponse<object>.Fail(string.Join(", ", validacion.Errors.Select(e => e.ErrorMessage)));

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var dispositivo = Request.Headers["User-Agent"].ToString();

            var resultado = await _authService.LoginAsync(request, ip, dispositivo);
            return ApiResponse<object>.Ok(resultado, "Inicio de sesión exitoso.");
        }

        [HttpPost("refresh")]
        public async Task<ApiResponse<object>> Refresh([FromBody] RefreshTokenRequest request)
        {
            var validacion = await _refreshValidator.ValidateAsync(request);
            if (!validacion.IsValid)
                return ApiResponse<object>.Fail(string.Join(", ", validacion.Errors.Select(e => e.ErrorMessage)));

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var dispositivo = Request.Headers["User-Agent"].ToString();

            var resultado = await _authService.RefreshTokenAsync(request, ip, dispositivo);
            return ApiResponse<object>.Ok(resultado, "Token renovado exitosamente.");
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ApiResponse<object>> Logout([FromBody] RefreshTokenRequest request)
        {
            var validacion = await _refreshValidator.ValidateAsync(request);
            if (!validacion.IsValid)
                return ApiResponse<object>.Fail(string.Join(", ", validacion.Errors.Select(e => e.ErrorMessage)));

            var correo = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            await _authService.LogoutAsync(request, correo);
            return ApiResponse<object>.Ok("Sesión cerrada exitosamente.");
        }
    }
}
