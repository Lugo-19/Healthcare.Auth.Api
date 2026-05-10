using System.Net;

namespace Healthcare.Auth.Api.Shared.Commons.Exceptions
{
    /// <summary>
    /// Lanzar cuando el usuario está autenticado pero no tiene permisos para el recurso. Retorna HTTP 403.
    /// </summary>
    public class ForbiddenException : BaseApiException
    {
        public ForbiddenException(string publicMessage, string? internalDetail = null): base(HttpStatusCode.Forbidden, publicMessage, internalDetail) { }
    }
}
