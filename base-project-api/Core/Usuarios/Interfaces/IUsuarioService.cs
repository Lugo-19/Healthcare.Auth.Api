using Healthcare.Auth.Api.Core.Usuarios.Dtos.Request;
using Healthcare.Auth.Api.Core.Usuarios.Dtos.Response;
using Healthcare.Auth.Api.Shared.Commons;

namespace Healthcare.Auth.Api.Core.Usuarios.Interfaces
{
    public interface IUsuarioService
    {
        Task<PaginatedResponse<UsuarioResponse>> ListarAsync(int page, int pageSize, string? filtro, short? idEstado);
        Task<UsuarioResponse> ObtenerPorIdAsync(Guid id);
        Task<Guid> CrearAsync(CrearUsuarioRequest request, string usuarioCreacion);
        Task ActualizarAsync(Guid id, ActualizarUsuarioRequest request, string usuarioModificacion);
        Task EliminarAsync(Guid id, string usuarioModificacion);
        Task CambiarEstadoAsync(Guid id, CambiarEstadoUsuarioRequest request, string usuarioModificacion);
    }
}
