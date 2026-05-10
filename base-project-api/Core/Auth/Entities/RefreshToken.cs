namespace Healthcare.Auth.Api.Core.Auth.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid IdUsuario { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
        public DateTime? FechaRevocacion { get; set; }
        public short IdEstado { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioApellido { get; set; } = string.Empty;
        public string UsuarioCorreo { get; set; } = string.Empty;
        public Guid? UsuarioIdRol { get; set; }
        public string? UsuarioNombreRol { get; set; }
        public short UsuarioIdEstado { get; set; }

        public bool EsValido => FechaRevocacion == null
                             && FechaExpiracion > DateTime.UtcNow
                             && IdEstado == 1;
    }
}
