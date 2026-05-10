using Asp.Versioning;
using FluentValidation;
using Healthcare.Auth.Api.Core.Roles.Dtos.Request;
using Healthcare.Auth.Api.Core.Roles.Interfaces;
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
    public class RolesController : ControllerBase
    {
        private readonly IRolService _service;
        private readonly IValidator<CrearRolRequest> _crearValidator;
        private readonly IValidator<ActualizarRolRequest> _actualizarValidator;
        private readonly IValidator<AsignarPermisosRequest> _asignarValidator;

        public RolesController(
            IRolService service,
            IValidator<CrearRolRequest> crearValidator,
            IValidator<ActualizarRolRequest> actualizarValidator,
            IValidator<AsignarPermisosRequest> asignarValidator)
        {
            _service = service;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
            _asignarValidator = asignarValidator;
        }

        private string UsuarioActual => User.FindFirstValue(ClaimTypes.Email) ?? "SYSTEM";

        [HttpGet]
        public async Task<ApiResponse<object>> Listar([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string? filtro = null)
            => ApiResponse<object>.Ok(await _service.ListarAsync(page, pageSize, filtro));

        [HttpGet("{id:guid}")]
        public async Task<ApiResponse<object>> ObtenerPorId(Guid id)
            => ApiResponse<object>.Ok(await _service.ObtenerPorIdAsync(id));

        [HttpPost]
        public async Task<ApiResponse<object>> Crear([FromBody] CrearRolRequest request)
        {
            var v = await _crearValidator.ValidateAsync(request);
            if (!v.IsValid) return ApiResponse<object>.Fail(string.Join(", ", v.Errors.Select(e => e.ErrorMessage)));

            var id = await _service.CrearAsync(request, UsuarioActual);
            return ApiResponse<object>.Ok(new { id }, "Rol creado exitosamente.");
        }

        [HttpPut("{id:guid}")]
        public async Task<ApiResponse<object>> Actualizar(Guid id, [FromBody] ActualizarRolRequest request)
        {
            var v = await _actualizarValidator.ValidateAsync(request);
            if (!v.IsValid) return ApiResponse<object>.Fail(string.Join(", ", v.Errors.Select(e => e.ErrorMessage)));

            await _service.ActualizarAsync(id, request, UsuarioActual);
            return ApiResponse<object>.Ok("Rol actualizado exitosamente.");
        }

        [HttpDelete("{id:guid}")]
        public async Task<ApiResponse<object>> Eliminar(Guid id)
        {
            await _service.EliminarAsync(id, UsuarioActual);
            return ApiResponse<object>.Ok("Rol eliminado exitosamente.");
        }

        [HttpGet("{id:guid}/permisos")]
        public async Task<ApiResponse<object>> ListarPermisos(Guid id)
            => ApiResponse<object>.Ok(await _service.ListarPermisosAsync(id));

        [HttpPut("{id:guid}/permisos")]
        public async Task<ApiResponse<object>> AsignarPermisos(Guid id, [FromBody] AsignarPermisosRequest request)
        {
            var v = await _asignarValidator.ValidateAsync(request);
            if (!v.IsValid) return ApiResponse<object>.Fail(string.Join(", ", v.Errors.Select(e => e.ErrorMessage)));

            await _service.AsignarPermisosAsync(id, request, UsuarioActual);
            return ApiResponse<object>.Ok("Permisos asignados al rol.");
        }
    }
}
