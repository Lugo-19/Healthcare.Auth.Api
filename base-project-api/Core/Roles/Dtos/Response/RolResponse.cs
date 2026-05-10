namespace Healthcare.Auth.Api.Core.Roles.Dtos.Response
{
    public class RolResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public short IdEstado { get; set; }
        public string? NombreEstado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; } = string.Empty;
        public DateTime? FechaModificacion { get; set; }
        public string? UsuarioModificacion { get; set; }
    }

    public class RolListResponse : RolResponse
    {
        public long TotalRecords { get; set; }
    }

    public class PermisoDeRolResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Modulo { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
    }
}
