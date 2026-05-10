using Healthcare.Auth.Api.Core.Roles.Dtos.Response;

namespace Healthcare.Auth.Api.Core.Roles.Interfaces
{
    public interface IRolRepository
    {
        Task<IEnumerable<RolListResponse>> ListarAsync(int page, int pageSize, string? filtro);
        Task<RolResponse?> ObtenerPorIdAsync(Guid id);
        Task<Guid> CrearAsync(string nombre, string? descripcion, string usuarioCreacion);
        Task ActualizarAsync(Guid id, string nombre, string? descripcion, string usuarioModificacion);
        Task EliminarAsync(Guid id, string usuarioModificacion);
        Task<IEnumerable<PermisoDeRolResponse>> ListarPermisosDeRolAsync(Guid idRol);
        Task AsignarPermisosAsync(Guid idRol, Guid[] idsPermisos, string usuarioCreacion);
    }
}
