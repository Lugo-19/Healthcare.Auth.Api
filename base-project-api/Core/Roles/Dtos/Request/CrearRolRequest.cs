namespace Healthcare.Auth.Api.Core.Roles.Dtos.Request
{
    public class CrearRolRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class ActualizarRolRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class AsignarPermisosRequest
    {
        public List<Guid> IdsPermisos { get; set; } = [];
    }
}
