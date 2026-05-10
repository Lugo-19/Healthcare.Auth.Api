using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Healthcare.Auth.Api.Shared.Helpers
{
    public class HttpExecutor
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private static readonly JsonSerializerOptions JsonOptions = new(){ PropertyNameCaseInsensitive = true };

        public HttpExecutor(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Realiza una petición GET y retorna el resultado deserializado.
        /// Retorna null si la respuesta no tiene contenido o no fue exitosa.
        /// </summary>
        /// <typeparam name="TResponse">Tipo al que se deserializa la respuesta.</typeparam>
        /// <param name="url">URL del endpoint.</param>
        /// <param name="queryParams">Parámetros de consulta opcionales que se agregan a la URL.</param>
        /// <param name="headers">Cabeceras HTTP opcionales (ej: Authorization).</param>
        public async Task<TResponse?> GetAsync<TResponse>( string url,Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null)
        {
            var client = BuildClient(headers);
            var fullUrl = BuildUrl(url, queryParams);
            var response = await client.GetAsync(fullUrl);
            return await DeserializeResponse<TResponse>(response);
        }

        /// <summary>
        /// Realiza una petición POST con un body JSON y retorna el resultado deserializado.
        /// Retorna null si la respuesta no tiene contenido o no fue exitosa.
        /// </summary>
        /// <typeparam name="TResponse">Tipo al que se deserializa la respuesta.</typeparam>
        /// <param name="url">URL del endpoint.</param>
        /// <param name="body">Objeto que se serializa como JSON en el body.</param>
        /// <param name="queryParams">Parámetros de consulta opcionales que se agregan a la URL.</param>
        /// <param name="headers">Cabeceras HTTP opcionales (ej: Authorization).</param>
        public async Task<TResponse?> PostAsync<TResponse>(string url, object body, Dictionary<string, string>? queryParams = null,Dictionary<string, string>? headers = null)
        {
            var client = BuildClient(headers);
            var fullUrl = BuildUrl(url, queryParams);
            var response = await client.PostAsync(fullUrl, BuildJsonContent(body));
            return await DeserializeResponse<TResponse>(response);
        }

        /// <summary>
        /// Realiza una petición POST sin esperar contenido en la respuesta.
        /// Útil para endpoints que solo retornan código de estado.
        /// </summary>
        /// <param name="url">URL del endpoint.</param>
        /// <param name="body">Objeto que se serializa como JSON en el body.</param>
        /// <param name="queryParams">Parámetros de consulta opcionales que se agregan a la URL.</param>
        /// <param name="headers">Cabeceras HTTP opcionales (ej: Authorization).</param>
        public async Task<bool> PostAsync(string url,object body,Dictionary<string, string>? queryParams = null,Dictionary<string, string>? headers = null)
        {
            var client = BuildClient(headers);
            var fullUrl = BuildUrl(url, queryParams);
            var response = await client.PostAsync(fullUrl, BuildJsonContent(body));
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Realiza una petición PUT con un body JSON y retorna el resultado deserializado.
        /// Retorna null si la respuesta no tiene contenido o no fue exitosa.
        /// </summary>
        /// <typeparam name="TResponse">Tipo al que se deserializa la respuesta.</typeparam>
        /// <param name="url">URL del endpoint.</param>
        /// <param name="body">Objeto que se serializa como JSON en el body.</param>
        /// <param name="queryParams">Parámetros de consulta opcionales que se agregan a la URL.</param>
        /// <param name="headers">Cabeceras HTTP opcionales (ej: Authorization).</param>
        public async Task<TResponse?> PutAsync<TResponse>(string url,object body,Dictionary<string, string>? queryParams = null,Dictionary<string, string>? headers = null)
        {
            var client = BuildClient(headers);
            var fullUrl = BuildUrl(url, queryParams);
            var response = await client.PutAsync(fullUrl, BuildJsonContent(body));
            return await DeserializeResponse<TResponse>(response);
        }

        /// <summary>
        /// Realiza una petición PUT sin esperar contenido en la respuesta.
        /// Útil para endpoints que solo retornan código de estado.
        /// </summary>
        /// <param name="url">URL del endpoint.</param>
        /// <param name="body">Objeto que se serializa como JSON en el body.</param>
        /// <param name="queryParams">Parámetros de consulta opcionales que se agregan a la URL.</param>
        /// <param name="headers">Cabeceras HTTP opcionales (ej: Authorization).</param>
        public async Task<bool> PutAsync(string url,object body,Dictionary<string, string>? queryParams = null,Dictionary<string, string>? headers = null)
        {
            var client = BuildClient(headers);
            var fullUrl = BuildUrl(url, queryParams);
            var response = await client.PutAsync(fullUrl, BuildJsonContent(body));
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Realiza una petición DELETE y retorna el resultado deserializado.
        /// Retorna null si la respuesta no tiene contenido o no fue exitosa.
        /// </summary>
        /// <typeparam name="TResponse">Tipo al que se deserializa la respuesta.</typeparam>
        /// <param name="url">URL del endpoint.</param>
        /// <param name="queryParams">Parámetros de consulta opcionales que se agregan a la URL.</param>
        /// <param name="headers">Cabeceras HTTP opcionales (ej: Authorization).</param>
        public async Task<TResponse?> DeleteAsync<TResponse>(string url,Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null)
        {
            var client = BuildClient(headers);
            var fullUrl = BuildUrl(url, queryParams);
            var response = await client.DeleteAsync(fullUrl);
            return await DeserializeResponse<TResponse>(response);
        }

        /// <summary>
        /// Realiza una petición DELETE sin esperar contenido en la respuesta.
        /// Útil para endpoints que solo retornan código de estado.
        /// </summary>
        /// <param name="url">URL del endpoint.</param>
        /// <param name="queryParams">Parámetros de consulta opcionales que se agregan a la URL.</param>
        /// <param name="headers">Cabeceras HTTP opcionales (ej: Authorization).</param>
        public async Task<bool> DeleteAsync(string url,Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null)
        {
            var client = BuildClient(headers);
            var fullUrl = BuildUrl(url, queryParams);
            var response = await client.DeleteAsync(fullUrl);
            return response.IsSuccessStatusCode;
        }

        private HttpClient BuildClient(Dictionary<string, string>? headers)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (headers is null) return client;

            foreach (var (key, value) in headers)
                client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);

            return client;
        }

        private static string BuildUrl(string url, Dictionary<string, string>? queryParams)
        {
            if (queryParams is null || queryParams.Count == 0) return url;
            var query = string.Join("&", queryParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
            return $"{url}?{query}";
        }

        private static StringContent BuildJsonContent(object body) =>new(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        private static async Task<TResponse?> DeserializeResponse<TResponse>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode) return default;
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content)) return default;
            return JsonSerializer.Deserialize<TResponse>(content, JsonOptions);
        }
    }
}
