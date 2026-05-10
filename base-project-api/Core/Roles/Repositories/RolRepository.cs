using Healthcare.Auth.Api.Core.Roles.Dtos.Response;
using Healthcare.Auth.Api.Core.Roles.Interfaces;
using Healthcare.Auth.Api.Shared.Helpers;

namespace Healthcare.Auth.Api.Core.Roles.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly DbExecutor _db;

        public RolRepository(DbExecutor db)
        {
            _db = db;
        }

        public async Task<IEnumerable<RolListResponse>> ListarAsync(int page, int pageSize, string? filtro)
            => await _db.QueryAsync<RolListResponse>("auth.sp_listar_roles", new
            {
                p_page = page,
                p_page_size = pageSize,
                p_filtro = filtro
            });

        public async Task<RolResponse?> ObtenerPorIdAsync(Guid id)
            => await _db.QuerySingleAsync<RolResponse>("auth.sp_obtener_rol_por_id", new { p_id = id });

        public async Task<Guid> CrearAsync(string nombre, string? descripcion, string usuarioCreacion)
            => await _db.ExecuteScalarAsync<Guid>("auth.sp_crear_rol", new
            {
                p_nombre = nombre,
                p_descripcion = descripcion,
                p_usuario_creacion = usuarioCreacion
            });

        public async Task ActualizarAsync(Guid id, string nombre, string? descripcion, string usuarioModificacion)
            => await _db.ExecuteAsync("auth.sp_actualizar_rol", new
            {
                p_id = id,
                p_nombre = nombre,
                p_descripcion = descripcion,
                p_usuario_modificacion = usuarioModificacion
            });

        public async Task EliminarAsync(Guid id, string usuarioModificacion)
            => await _db.ExecuteAsync("auth.sp_eliminar_rol", new { p_id = id, p_usuario_modificacion = usuarioModificacion });

        public async Task<IEnumerable<PermisoDeRolResponse>> ListarPermisosDeRolAsync(Guid idRol)
            => await _db.QueryAsync<PermisoDeRolResponse>("auth.sp_listar_permisos_de_rol", new { p_id_rol = idRol });

        public async Task AsignarPermisosAsync(Guid idRol, Guid[] idsPermisos, string usuarioCreacion)
            => await _db.ExecuteAsync("auth.sp_asignar_permisos_a_rol", new
            {
                p_id_rol = idRol,
                p_ids_permisos = idsPermisos,
                p_usuario_creacion = usuarioCreacion
            });
    }
}
