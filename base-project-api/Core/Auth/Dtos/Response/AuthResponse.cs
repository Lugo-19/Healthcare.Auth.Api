namespace Healthcare.Auth.Api.Core.Auth.Dtos.Response
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expiracion { get; set; }
        public UsuarioInfo Usuario { get; set; } = new();
    }

    public class UsuarioInfo
    {
        public Guid Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? Rol { get; set; }
    }
}
