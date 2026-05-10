using Asp.Versioning;
using FluentValidation;
using Healthcare.Auth.Api.Core.Usuarios.Dtos.Request;
using Healthcare.Auth.Api.Core.Usuarios.Interfaces;
using Healthcare.Auth.Api.Shared.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Healthcare.Auth.Api.Controllers.v1
{
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _service;
        private readonly IValidator<CrearUsuarioRequest> _crearValidator;
        private readonly IValidator<ActualizarUsuarioRequest> _actualizarValidator;
        private readonly IValidator<CambiarEstadoUsuarioRequest> _estadoValidator;

        public UsuariosController(
            IUsuarioService service,
            IValidator<CrearUsuarioRequest> crearValidator,
            IValidator<ActualizarUsuarioRequest> actualizarValidator,
            IValidator<CambiarEstadoUsuarioRequest> estadoValidator)
        {
            _service = service;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
            _estadoValidator = estadoValidator;
        }

        private string UsuarioActual => User.FindFirstValue(ClaimTypes.Email) ?? "SYSTEM";

        [HttpGet]
        public async Task<ApiResponse<object>> Listar([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string? filtro = null, [FromQuery] short? idEstado = null)
        {
            var resultado = await _service.ListarAsync(page, pageSize, filtro, idEstado);
            return ApiResponse<object>.Ok(resultado);
        }

        [HttpGet("{id:guid}")]
        public async Task<ApiResponse<object>> ObtenerPorId(Guid id)
        {
            var resultado = await _service.ObtenerPorIdAsync(id);
            return ApiResponse<object>.Ok(resultado);
        }

        [HttpPost]
        public async Task<ApiResponse<object>> Crear([FromBody] CrearUsuarioRequest request)
        {
            var v = await _crearValidator.ValidateAsync(request);
            if (!v.IsValid) return ApiResponse<object>.Fail(string.Join(", ", v.Errors.Select(e => e.ErrorMessage)));

            var id = await _service.CrearAsync(request, UsuarioActual);
            return ApiResponse<object>.Ok(new { id }, "Usuario creado exitosamente.");
        }

        [HttpPut("{id:guid}")]
        public async Task<ApiResponse<object>> Actualizar(Guid id, [FromBody] ActualizarUsuarioRequest request)
        {
            var v = await _actualizarValidator.ValidateAsync(request);
            if (!v.IsValid) return ApiResponse<object>.Fail(string.Join(", ", v.Errors.Select(e => e.ErrorMessage)));

            await _service.ActualizarAsync(id, request, UsuarioActual);
            return ApiResponse<object>.Ok("Usuario actualizado exitosamente.");
        }

        [HttpDelete("{id:guid}")]
        public async Task<ApiResponse<object>> Eliminar(Guid id)
        {
            await _service.EliminarAsync(id, UsuarioActual);
            return ApiResponse<object>.Ok("Usuario eliminado exitosamente.");
        }

        [HttpPut("{id:guid}/estado")]
        public async Task<ApiResponse<object>> CambiarEstado(Guid id, [FromBody] CambiarEstadoUsuarioRequest request)
        {
            var v = await _estadoValidator.ValidateAsync(request);
            if (!v.IsValid) return ApiResponse<object>.Fail(string.Join(", ", v.Errors.Select(e => e.ErrorMessage)));

            await _service.CambiarEstadoAsync(id, request, UsuarioActual);
            return ApiResponse<object>.Ok("Estado del usuario actualizado.");
        }
    }
}
