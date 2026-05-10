using Healthcare.Auth.Api.Core.Roles.Dtos.Request;
using Healthcare.Auth.Api.Core.Roles.Dtos.Response;
using Healthcare.Auth.Api.Core.Roles.Interfaces;
using Healthcare.Auth.Api.Shared.Commons;
using Healthcare.Auth.Api.Shared.Commons.Exceptions;

namespace Healthcare.Auth.Api.Core.Roles.Services
{
    public class RolService : IRolService
    {
        private readonly IRolRepository _repository;

        public RolService(IRolRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<RolResponse>> ListarAsync(int page, int pageSize, string? filtro)
        {
            var resultado = (await _repository.ListarAsync(page, pageSize, filtro)).ToList();
            var total = resultado.FirstOrDefault()?.TotalRecords ?? 0;
            var datos = resultado.Cast<RolResponse>().ToList();
            return PaginatedResponse<RolResponse>.Create(datos, page, pageSize, (int)total);
        }

        public async Task<RolResponse> ObtenerPorIdAsync(Guid id)
            => await _repository.ObtenerPorIdAsync(id)
               ?? throw new NotFoundException("El rol solicitado no existe.");

        public async Task<Guid> CrearAsync(CrearRolRequest request, string usuarioCreacion)
            => await _repository.CrearAsync(request.Nombre, request.Descripcion, usuarioCreacion);

        public async Task ActualizarAsync(Guid id, ActualizarRolRequest request, string usuarioModificacion)
        {
            _ = await _repository.ObtenerPorIdAsync(id) ?? throw new NotFoundException("El rol solicitado no existe.");
            await _repository.ActualizarAsync(id, request.Nombre, request.Descripcion, usuarioModificacion);
        }

        public async Task EliminarAsync(Guid id, string usuarioModificacion)
        {
            _ = await _repository.ObtenerPorIdAsync(id) ?? throw new NotFoundException("El rol solicitado no existe.");
            await _repository.EliminarAsync(id, usuarioModificacion);
        }

        public async Task<IEnumerable<PermisoDeRolResponse>> ListarPermisosAsync(Guid idRol)
        {
            _ = await _repository.ObtenerPorIdAsync(idRol) ?? throw new NotFoundException("El rol solicitado no existe.");
            return await _repository.ListarPermisosDeRolAsync(idRol);
        }

        public async Task AsignarPermisosAsync(Guid idRol, AsignarPermisosRequest request, string usuarioCreacion)
        {
            _ = await _repository.ObtenerPorIdAsync(idRol) ?? throw new NotFoundException("El rol solicitado no existe.");
            await _repository.AsignarPermisosAsync(idRol, request.IdsPermisos.ToArray(), usuarioCreacion);
        }
    }
}
