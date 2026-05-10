using Healthcare.Auth.Api.Core.Permisos.Dtos.Request;
using Healthcare.Auth.Api.Core.Permisos.Dtos.Response;
using Healthcare.Auth.Api.Core.Permisos.Interfaces;
using Healthcare.Auth.Api.Shared.Commons;
using Healthcare.Auth.Api.Shared.Commons.Exceptions;

namespace Healthcare.Auth.Api.Core.Permisos.Services
{
    public class PermisoService : IPermisoService
    {
        private readonly IPermisoRepository _repository;

        public PermisoService(IPermisoRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<PermisoResponse>> ListarAsync(int page, int pageSize, string? filtro, string? modulo)
        {
            var resultado = (await _repository.ListarAsync(page, pageSize, filtro, modulo)).ToList();
            var total = resultado.FirstOrDefault()?.TotalRecords ?? 0;
            var datos = resultado.Cast<PermisoResponse>().ToList();
            return PaginatedResponse<PermisoResponse>.Create(datos, page, pageSize, (int)total);
        }

        public async Task<PermisoResponse> ObtenerPorIdAsync(Guid id)
            => await _repository.ObtenerPorIdAsync(id)
               ?? throw new NotFoundException("El permiso solicitado no existe.");

        public async Task<Guid> CrearAsync(CrearPermisoRequest request, string usuarioCreacion)
            => await _repository.CrearAsync(request.Nombre, request.Descripcion, request.Modulo, request.Accion, usuarioCreacion);

        public async Task ActualizarAsync(Guid id, ActualizarPermisoRequest request, string usuarioModificacion)
        {
            _ = await _repository.ObtenerPorIdAsync(id) ?? throw new NotFoundException("El permiso solicitado no existe.");
            await _repository.ActualizarAsync(id, request.Nombre, request.Descripcion, request.Modulo, request.Accion, usuarioModificacion);
        }

        public async Task EliminarAsync(Guid id, string usuarioModificacion)
        {
            _ = await _repository.ObtenerPorIdAsync(id) ?? throw new NotFoundException("El permiso solicitado no existe.");
            await _repository.EliminarAsync(id, usuarioModificacion);
        }
    }
}
