using Healthcare.Auth.Api.Core.Usuarios.Dtos.Response;

namespace Healthcare.Auth.Api.Core.Usuarios.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<UsuarioListResponse>> ListarAsync(int page, int pageSize, string? filtro, short? idEstado);
        Task<UsuarioResponse?> ObtenerPorIdAsync(Guid id);
        Task<Guid> CrearAsync(string nombre, string apellido, string correo, string contrasenaHash, Guid idRol, string usuarioCreacion);
        Task ActualizarAsync(Guid id, string nombre, string apellido, string correo, Guid idRol, string usuarioModificacion);
        Task EliminarAsync(Guid id, string usuarioModificacion);
        Task CambiarEstadoAsync(Guid id, short idEstado, string usuarioModificacion);
    }
}
