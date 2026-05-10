using System.Net;

namespace Healthcare.Auth.Api.Shared.Commons.Exceptions
{
    /// <summary>
    /// Lanzar cuando existe un conflicto con el estado actual del recurso (ej: duplicado). Retorna HTTP 409.
    /// </summary>
    public class ConflictException : BaseApiException
    {
        public ConflictException(string publicMessage, string? internalDetail = null) : base(HttpStatusCode.Conflict, publicMessage, internalDetail) { }
    }
}
