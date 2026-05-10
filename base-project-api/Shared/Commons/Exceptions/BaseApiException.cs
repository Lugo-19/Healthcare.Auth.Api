using System.Net;

namespace Healthcare.Auth.Api.Shared.Commons.Exceptions
{
    public abstract class BaseApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string PublicMessage { get; }

        protected BaseApiException(HttpStatusCode statusCode, string publicMessage, string? internalDetail = null) : base(internalDetail ?? publicMessage)
        {
            StatusCode = statusCode;
            PublicMessage = publicMessage;
        }
    }
}
