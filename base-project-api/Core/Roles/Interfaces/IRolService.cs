using Healthcare.Auth.Api.Core.Roles.Dtos.Request;
using Healthcare.Auth.Api.Core.Roles.Dtos.Response;
using Healthcare.Auth.Api.Shared.Commons;

namespace Healthcare.Auth.Api.Core.Roles.Interfaces
{
    public interface IRolService
    {
        Task<PaginatedResponse<RolResponse>> ListarAsync(int page, int pageSize, string? filtro);
        Task<RolResponse> ObtenerPorIdAsync(Guid id);
        Task<Guid> CrearAsync(CrearRolRequest request, string usuarioCreacion);
        Task ActualizarAsync(Guid id, ActualizarRolRequest request, string usuarioModificacion);
        Task EliminarAsync(Guid id, string usuarioModificacion);
        Task<IEnumerable<PermisoDeRolResponse>> ListarPermisosAsync(Guid idRol);
        Task AsignarPermisosAsync(Guid idRol, AsignarPermisosRequest request, string usuarioCreacion);
    }
}
