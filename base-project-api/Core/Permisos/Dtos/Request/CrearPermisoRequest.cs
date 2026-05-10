namespace Healthcare.Auth.Api.Core.Permisos.Dtos.Request
{
    public class CrearPermisoRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Modulo { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
    }

    public class ActualizarPermisoRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Modulo { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
    }
}
