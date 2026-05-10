using System.Net;

namespace Healthcare.Auth.Api.Shared.Commons.Exceptions
{
    /// <summary>
    /// Lanzar cuando el usuario no está autenticado o el token es inválido. Retorna HTTP 401.
    /// </summary>
    public class UnauthorizedException : BaseApiException
    {
        public UnauthorizedException(string publicMessage, string? internalDetail = null) : base(HttpStatusCode.Unauthorized, publicMessage, internalDetail) { }
    }
}
