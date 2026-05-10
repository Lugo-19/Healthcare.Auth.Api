namespace Healthcare.Auth.Api.Shared.Constants
{
    public static class ApiMessages
    {
        public const string Success = "Solicitud procesada correctamente.";
        public const string InternalServerError = "Ha ocurrido un error interno en el servidor.";
        public const string NotFound = "El recurso solicitado no fue encontrado.";
        public const string BadRequest = "La solicitud contiene datos inválidos.";
        public const string Unauthorized = "No está autenticado para realizar esta acción.";
        public const string Forbidden = "No tiene permisos para realizar esta acción.";
        public const string Conflict = "Ya existe un recurso con los datos proporcionados.";
    }
}
