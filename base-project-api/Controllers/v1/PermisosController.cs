using Asp.Versioning;
using FluentValidation;
using Healthcare.Auth.Api.Core.Permisos.Dtos.Request;
using Healthcare.Auth.Api.Core.Permisos.Interfaces;
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
    public class PermisosController : ControllerBase
    {
        private readonly IPermisoService _service;
        private readonly IValidator<CrearPermisoRequest> _crearValidator;
        private readonly IValidator<ActualizarPermisoRequest> _actualizarValidator;

        public PermisosController(
            IPermisoService service,
            IValidator<CrearPermisoRequest> crearValidator,
            IValidator<ActualizarPermisoRequest> actualizarValidator)
        {
            _service = service;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
        }

        private string UsuarioActual => User.FindFirstValue(ClaimTypes.Email) ?? "SYSTEM";

        [HttpGet]
        public async Task<ApiResponse<object>> Listar([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string? filtro = null, [FromQuery] string? modulo = null)
            => ApiResponse<object>.Ok(await _service.ListarAsync(page, pageSize, filtro, modulo));

        [HttpGet("{id:guid}")]
        public async Task<ApiResponse<object>> ObtenerPorId(Guid id)
            => ApiResponse<object>.Ok(await _service.ObtenerPorIdAsync(id));

        [HttpPost]
        public async Task<ApiResponse<object>> Crear([FromBody] CrearPermisoRequest request)
        {
            var v = await _crearValidator.ValidateAsync(request);
            if (!v.IsValid) return ApiResponse<object>.Fail(string.Join(", ", v.Errors.Select(e => e.ErrorMessage)));

            var id = await _service.CrearAsync(request, UsuarioActual);
            return ApiResponse<object>.Ok(new { id }, "Permiso creado exitosamente.");
        }

        [HttpPut("{id:guid}")]
        public async Task<ApiResponse<object>> Actualizar(Guid id, [FromBody] ActualizarPermisoRequest request)
        {
            var v = await _actualizarValidator.ValidateAsync(request);
            if (!v.IsValid) return ApiResponse<object>.Fail(string.Join(", ", v.Errors.Select(e => e.ErrorMessage)));

            await _service.ActualizarAsync(id, request, UsuarioActual);
            return ApiResponse<object>.Ok("Permiso actualizado exitosamente.");
        }

        [HttpDelete("{id:guid}")]
        public async Task<ApiResponse<object>> Eliminar(Guid id)
        {
            await _service.EliminarAsync(id, UsuarioActual);
            return ApiResponse<object>.Ok("Permiso eliminado exitosamente.");
        }
    }
}
