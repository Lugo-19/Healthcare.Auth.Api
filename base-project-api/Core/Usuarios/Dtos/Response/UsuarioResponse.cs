namespace Healthcare.Auth.Api.Core.Usuarios.Dtos.Response
{
    public class UsuarioResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public Guid? IdRol { get; set; }
        public string? NombreRol { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public short IdEstado { get; set; }
        public string? NombreEstado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; } = string.Empty;
        public DateTime? FechaModificacion { get; set; }
        public string? UsuarioModificacion { get; set; }
    }

    public class UsuarioListResponse : UsuarioResponse
    {
        public long TotalRecords { get; set; }
    }
}
