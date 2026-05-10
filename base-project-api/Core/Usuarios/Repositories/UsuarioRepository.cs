using Healthcare.Auth.Api.Core.Usuarios.Dtos.Response;
using Healthcare.Auth.Api.Core.Usuarios.Interfaces;
using Healthcare.Auth.Api.Shared.Helpers;

namespace Healthcare.Auth.Api.Core.Usuarios.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DbExecutor _db;

        public UsuarioRepository(DbExecutor db)
        {
            _db = db;
        }

        public async Task<IEnumerable<UsuarioListResponse>> ListarAsync(int page, int pageSize, string? filtro, short? idEstado)
            => await _db.QueryAsync<UsuarioListResponse>("auth.sp_listar_usuarios", new
            {
                p_page = page,
                p_page_size = pageSize,
                p_filtro = filtro,
                p_id_estado = idEstado
            });

        public async Task<UsuarioResponse?> ObtenerPorIdAsync(Guid id)
            => await _db.QuerySingleAsync<UsuarioResponse>("auth.sp_obtener_usuario_por_id", new { p_id = id });

        public async Task<Guid> CrearAsync(string nombre, string apellido, string correo, string contrasenaHash, Guid idRol, string usuarioCreacion)
            => await _db.ExecuteScalarAsync<Guid>("auth.sp_crear_usuario", new
            {
                p_nombre = nombre,
                p_apellido = apellido,
                p_correo = correo,
                p_contrasena = contrasenaHash,
                p_id_rol = idRol,
                p_usuario_creacion = usuarioCreacion
            });

        public async Task ActualizarAsync(Guid id, string nombre, string apellido, string correo, Guid idRol, string usuarioModificacion)
            => await _db.ExecuteAsync("auth.sp_actualizar_usuario", new
            {
                p_id = id,
                p_nombre = nombre,
                p_apellido = apellido,
                p_correo = correo,
                p_id_rol = idRol,
                p_usuario_modificacion = usuarioModificacion
            });

        public async Task EliminarAsync(Guid id, string usuarioModificacion)
            => await _db.ExecuteAsync("auth.sp_eliminar_usuario", new { p_id = id, p_usuario_modificacion = usuarioModificacion });

        public async Task CambiarEstadoAsync(Guid id, short idEstado, string usuarioModificacion)
            => await _db.ExecuteAsync("auth.sp_cambiar_estado_usuario", new
            {
                p_id = id,
                p_id_estado = idEstado,
                p_usuario_modificacion = usuarioModificacion
            });
    }
}
