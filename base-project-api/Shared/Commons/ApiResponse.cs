using Healthcare.Auth.Api.Shared.Constants;

namespace Healthcare.Auth.Api.Shared.Commons
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string? ErrorDetail { get; set; }

        /// <summary>
        /// Respuesta exitosa con datos y mensaje opcional.
        /// </summary>
        public static ApiResponse<T> Ok(T data, string message = ApiMessages.Success) => new()
        {
            Success = true,
            Message = message,
            Data = data
        };

        /// <summary>
        /// Respuesta exitosa sin datos (para operaciones que no retornan resultado).
        /// </summary>
        public static ApiResponse<T> Ok(string message = ApiMessages.Success) => new()
        {
            Success = true,
            Message = message
        };

        /// <summary>
        /// Respuesta de error con mensaje público para el usuario y detalle interno del error real.
        /// </summary>
        /// <param name="message">Mensaje visible al público (no exponer detalles técnicos).</param>
        /// <param name="errorDetail">Mensaje interno con el error real, útil para logs y debugging.</param>
        public static ApiResponse<T> Fail(string message, string? errorDetail = null) => new()
        {
            Success = false,
            Message = message,
            ErrorDetail = errorDetail
        };
    }
}
