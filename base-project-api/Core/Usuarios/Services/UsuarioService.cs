using Healthcare.Auth.Api.Core.Usuarios.Dtos.Request;
using Healthcare.Auth.Api.Core.Usuarios.Dtos.Response;
using Healthcare.Auth.Api.Core.Usuarios.Interfaces;
using Healthcare.Auth.Api.Shared.Commons;
using Healthcare.Auth.Api.Shared.Commons.Exceptions;

namespace Healthcare.Auth.Api.Core.Usuarios.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<UsuarioResponse>> ListarAsync(int page, int pageSize, string? filtro, short? idEstado)
        {
            var resultado = (await _repository.ListarAsync(page, pageSize, filtro, idEstado)).ToList();
            var total = resultado.FirstOrDefault()?.TotalRecords ?? 0;
            var datos = resultado.Cast<UsuarioResponse>().ToList();
            return PaginatedResponse<UsuarioResponse>.Create(datos, page, pageSize, (int)total);
        }

        public async Task<UsuarioResponse> ObtenerPorIdAsync(Guid id)
            => await _repository.ObtenerPorIdAsync(id)
               ?? throw new NotFoundException("El usuario solicitado no existe.");

        public async Task<Guid> CrearAsync(CrearUsuarioRequest request, string usuarioCreacion)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(request.Contrasena, 11);
            return await _repository.CrearAsync(request.Nombre, request.Apellido, request.Correo, hash, request.IdRol, usuarioCreacion);
        }

        public async Task ActualizarAsync(Guid id, ActualizarUsuarioRequest request, string usuarioModificacion)
        {
            _ = await _repository.ObtenerPorIdAsync(id)
                ?? throw new NotFoundException("El usuario solicitado no existe.");
            await _repository.ActualizarAsync(id, request.Nombre, request.Apellido, request.Correo, request.IdRol, usuarioModificacion);
        }

        public async Task EliminarAsync(Guid id, string usuarioModificacion)
        {
            _ = await _repository.ObtenerPorIdAsync(id)
                ?? throw new NotFoundException("El usuario solicitado no existe.");
            await _repository.EliminarAsync(id, usuarioModificacion);
        }

        public async Task CambiarEstadoAsync(Guid id, CambiarEstadoUsuarioRequest request, string usuarioModificacion)
        {
            _ = await _repository.ObtenerPorIdAsync(id)
                ?? throw new NotFoundException("El usuario solicitado no existe.");
            await _repository.CambiarEstadoAsync(id, request.IdEstado, usuarioModificacion);
        }
    }
}
