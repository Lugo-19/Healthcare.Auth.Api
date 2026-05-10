using System.Net;

namespace Healthcare.Auth.Api.Shared.Commons.Exceptions
{
    /// <summary>
    /// Lanzar cuando los datos de entrada son inválidos o la solicitud no puede procesarse. Retorna HTTP 400.
    /// </summary>
    public class BadRequestException : BaseApiException
    {
        public BadRequestException(string publicMessage, string? internalDetail = null) : base(HttpStatusCode.BadRequest, publicMessage, internalDetail) { }
    }
}
