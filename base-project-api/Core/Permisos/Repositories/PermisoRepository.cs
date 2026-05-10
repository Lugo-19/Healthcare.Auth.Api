using Healthcare.Auth.Api.Core.Permisos.Dtos.Response;
using Healthcare.Auth.Api.Core.Permisos.Interfaces;
using Healthcare.Auth.Api.Shared.Helpers;

namespace Healthcare.Auth.Api.Core.Permisos.Repositories
{
    public class PermisoRepository : IPermisoRepository
    {
        private readonly DbExecutor _db;

        public PermisoRepository(DbExecutor db)
        {
            _db = db;
        }

        public async Task<IEnumerable<PermisoListResponse>> ListarAsync(int page, int pageSize, string? filtro, string? modulo)
            => await _db.QueryAsync<PermisoListResponse>("auth.sp_listar_permisos", new
            {
                p_page = page,
                p_page_size = pageSize,
                p_filtro = filtro,
                p_modulo = modulo
            });

        public async Task<PermisoResponse?> ObtenerPorIdAsync(Guid id)
            => await _db.QuerySingleAsync<PermisoResponse>("auth.sp_obtener_permiso_por_id", new { p_id = id });

        public async Task<Guid> CrearAsync(string nombre, string? descripcion, string modulo, string accion, string usuarioCreacion)
            => await _db.ExecuteScalarAsync<Guid>("auth.sp_crear_permiso", new
            {
                p_nombre = nombre,
                p_descripcion = descripcion,
                p_modulo = modulo,
                p_accion = accion,
                p_usuario_creacion = usuarioCreacion
            });

        public async Task ActualizarAsync(Guid id, string nombre, string? descripcion, string modulo, string accion, string usuarioModificacion)
            => await _db.ExecuteAsync("auth.sp_actualizar_permiso", new
            {
                p_id = id,
                p_nombre = nombre,
                p_descripcion = descripcion,
                p_modulo = modulo,
                p_accion = accion,
                p_usuario_modificacion = usuarioModificacion
            });

        public async Task EliminarAsync(Guid id, string usuarioModificacion)
            => await _db.ExecuteAsync("auth.sp_eliminar_permiso", new { p_id = id, p_usuario_modificacion = usuarioModificacion });
    }
}
