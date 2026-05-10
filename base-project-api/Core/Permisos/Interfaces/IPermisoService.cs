using Healthcare.Auth.Api.Core.Permisos.Dtos.Request;
using Healthcare.Auth.Api.Core.Permisos.Dtos.Response;
using Healthcare.Auth.Api.Shared.Commons;

namespace Healthcare.Auth.Api.Core.Permisos.Interfaces
{
    public interface IPermisoService
    {
        Task<PaginatedResponse<PermisoResponse>> ListarAsync(int page, int pageSize, string? filtro, string? modulo);
        Task<PermisoResponse> ObtenerPorIdAsync(Guid id);
        Task<Guid> CrearAsync(CrearPermisoRequest request, string usuarioCreacion);
        Task ActualizarAsync(Guid id, ActualizarPermisoRequest request, string usuarioModificacion);
        Task EliminarAsync(Guid id, string usuarioModificacion);
    }
}
