using System.Net;

namespace Healthcare.Auth.Api.Shared.Commons.Exceptions
{
    /// <summary>
    /// Lanzar cuando un recurso solicitado no existe. Retorna HTTP 404.
    /// </summary>
    public class NotFoundException : BaseApiException
    {
        public NotFoundException(string publicMessage, string? internalDetail = null) : base(HttpStatusCode.NotFound, publicMessage, internalDetail) { }
    }
}
