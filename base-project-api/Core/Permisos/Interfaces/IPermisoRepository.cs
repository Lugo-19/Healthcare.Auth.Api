using Healthcare.Auth.Api.Core.Permisos.Dtos.Response;

namespace Healthcare.Auth.Api.Core.Permisos.Interfaces
{
    public interface IPermisoRepository
    {
        Task<IEnumerable<PermisoListResponse>> ListarAsync(int page, int pageSize, string? filtro, string? modulo);
        Task<PermisoResponse?> ObtenerPorIdAsync(Guid id);
        Task<Guid> CrearAsync(string nombre, string? descripcion, string modulo, string accion, string usuarioCreacion);
        Task ActualizarAsync(Guid id, string nombre, string? descripcion, string modulo, string accion, string usuarioModificacion);
        Task EliminarAsync(Guid id, string usuarioModificacion);
    }
}
