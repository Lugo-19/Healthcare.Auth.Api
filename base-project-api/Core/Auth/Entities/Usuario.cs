namespace Healthcare.Auth.Api.Core.Auth.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public Guid? IdRol { get; set; }
        public string? NombreRol { get; set; }
        public short IntentosFallidos { get; set; }
        public DateTime? BloqueadoHasta { get; set; }
        public short IdEstado { get; set; }
    }
}
