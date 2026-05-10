namespace Healthcare.Auth.Api.Core.Usuarios.Dtos.Request
{
    public class ActualizarUsuarioRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public Guid IdRol { get; set; }
    }
}
