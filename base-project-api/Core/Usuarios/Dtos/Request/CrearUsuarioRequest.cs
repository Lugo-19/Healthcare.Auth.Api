namespace Healthcare.Auth.Api.Core.Usuarios.Dtos.Request
{
    public class CrearUsuarioRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public Guid IdRol { get; set; }
    }
}
