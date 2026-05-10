namespace Healthcare.Auth.Api.Core.Permisos.Dtos.Response
{
    public class PermisoResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Modulo { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public short IdEstado { get; set; }
        public string? NombreEstado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; } = string.Empty;
        public DateTime? FechaModificacion { get; set; }
        public string? UsuarioModificacion { get; set; }
    }

    public class PermisoListResponse : PermisoResponse
    {
        public long TotalRecords { get; set; }
    }
}
