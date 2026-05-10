using System.Data;
using System.Net;
using System.Net.Sockets;
using Dapper;
using Npgsql;

namespace Healthcare.Auth.Api.Shared.Helpers
{
    public class DbExecutor
    {
        private readonly NpgsqlDataSource _dataSource;

        public DbExecutor(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            var builder = new NpgsqlConnectionStringBuilder(connectionString);

            if (!IPAddress.TryParse(builder.Host, out _) && !string.IsNullOrEmpty(builder.Host))
            {
                // Extraer endpoint ID para Neon (requerido cuando se conecta por IP, ya que pierde SNI)
                var endpointId = builder.Host.Split('.').FirstOrDefault()?.Replace("-pooler", "");
                if (!string.IsNullOrEmpty(endpointId))
                {
                    builder.Options = string.IsNullOrEmpty(builder.Options)
                        ? $"endpoint={endpointId}"
                        : $"{builder.Options} endpoint={endpointId}";
                }

                try
                {
                    var addresses = Dns.GetHostAddresses(builder.Host);
                    var ipv4 = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                    if (ipv4 != null)
                    {
                        builder.Host = ipv4.ToString();
                    }
                }
                catch (SocketException ex)
                {
                    throw new InvalidOperationException(
                        $"No se pudo resolver el host de la base de datos: {builder.Host}.", ex);
                }
            }

            _dataSource = NpgsqlDataSource.Create(builder.ConnectionString);
        }

        private NpgsqlConnection CreateConnection() => _dataSource.CreateConnection();

        /// <summary>
        /// Construye SQL para llamar a una función de PostgreSQL: SELECT * FROM schema.func(@p1, @p2, ...).
        /// </summary>
        private static string BuildFunctionSql(string functionName, object? parameters)
        {
            if (parameters is null) return $"SELECT * FROM {functionName}()";

            var paramNames = parameters is DynamicParameters dp
                ? dp.ParameterNames.Select(n => $"@{n}")
                : parameters.GetType().GetProperties().Select(p => $"@{p.Name}");

            return $"SELECT * FROM {functionName}({string.Join(", ", paramNames)})";
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que no retorna datos (INSERT, UPDATE, DELETE).
        /// Retorna el número de filas afectadas.
        /// </summary>
        /// <param name="storedProcedure">Nombre del procedimiento almacenado.</param>
        /// <param name="parameters">Parámetros de entrada (objeto anónimo o DynamicParameters).</param>
        public async Task<int> ExecuteAsync(string storedProcedure, object? parameters = null)
        {
            await using var connection = CreateConnection();
            return await connection.ExecuteAsync(BuildFunctionSql(storedProcedure, parameters), parameters);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado y retorna una colección de registros.
        /// Usar cuando el SP devuelve múltiples filas.
        /// </summary>
        /// <typeparam name="T">Tipo al que se mapean los resultados.</typeparam>
        /// <param name="storedProcedure">Nombre del procedimiento almacenado.</param>
        /// <param name="parameters">Parámetros de entrada (objeto anónimo o DynamicParameters).</param>
        public async Task<IEnumerable<T>> QueryAsync<T>(string storedProcedure, object? parameters = null)
        {
            await using var connection = CreateConnection();
            return await connection.QueryAsync<T>(BuildFunctionSql(storedProcedure, parameters), parameters);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado y retorna un único registro.
        /// Retorna null si no se encontró ningún resultado.
        /// Usar cuando el SP devuelve como máximo una fila.
        /// </summary>
        /// <typeparam name="T">Tipo al que se mapea el resultado.</typeparam>
        /// <param name="storedProcedure">Nombre del procedimiento almacenado.</param>
        /// <param name="parameters">Parámetros de entrada (objeto anónimo o DynamicParameters).</param>
        public async Task<T?> QuerySingleAsync<T>(string storedProcedure, object? parameters = null)
        {
            await using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(BuildFunctionSql(storedProcedure, parameters), parameters);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado y retorna un valor escalar.
        /// Usar para obtener un único valor como COUNT, SUM o un ID generado.
        /// </summary>
        /// <typeparam name="T">Tipo del valor escalar retornado.</typeparam>
        /// <param name="storedProcedure">Nombre del procedimiento almacenado.</param>
        /// <param name="parameters">Parámetros de entrada (objeto anónimo o DynamicParameters).</param>
        public async Task<T?> ExecuteScalarAsync<T>(string storedProcedure, object? parameters = null)
        {
            await using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<T>(BuildFunctionSql(storedProcedure, parameters), parameters);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que retorna dos conjuntos de resultados.
        /// Usar cuando el SP hace dos SELECT y se necesitan ambos resultados.
        /// </summary>
        /// <typeparam name="T1">Tipo del primer conjunto de resultados.</typeparam>
        /// <typeparam name="T2">Tipo del segundo conjunto de resultados.</typeparam>
        /// <param name="storedProcedure">Nombre del procedimiento almacenado.</param>
        /// <param name="parameters">Parámetros de entrada (objeto anónimo o DynamicParameters).</param>
        public async Task<(IEnumerable<T1> First, IEnumerable<T2> Second)> QueryMultipleAsync<T1, T2>(string storedProcedure, object? parameters = null)
        {
            await using var connection = CreateConnection();
            await using var multi = await connection.QueryMultipleAsync(BuildFunctionSql(storedProcedure, parameters), parameters);
            var first = await multi.ReadAsync<T1>();
            var second = await multi.ReadAsync<T2>();

            return (first, second);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que retorna un listado de registros y parámetros de salida simultáneamente.
        /// Usar para paginación u otros casos donde el SP devuelve datos y valores calculados (ej: total de registros).
        /// Los parámetros de salida se leen desde el mismo DynamicParameters después de la ejecución.
        /// Ejemplo:
        ///   var parameters = new DynamicParameters();
        ///   parameters.Add("p_page", 1);
        ///   parameters.Add("p_total", dbType: DbType.Int32, direction: ParameterDirection.Output);
        ///   var records = await _db.QueryWithOutputAsync&lt;UserResponse&gt;("sp_get_users_paged", parameters);
        ///   int total = parameters.Get&lt;int&gt;("p_total");
        /// </summary>
        /// <typeparam name="T">Tipo al que se mapea el listado de resultados.</typeparam>
        /// <param name="storedProcedure">Nombre del procedimiento almacenado.</param>
        /// <param name="parameters">DynamicParameters con parámetros de entrada y salida configurados.</param>
        public async Task<IEnumerable<T>> QueryWithOutputAsync<T>(string storedProcedure, DynamicParameters parameters)
        {
            await using var connection = CreateConnection();
            return await connection.QueryAsync<T>(BuildFunctionSql(storedProcedure, parameters), parameters);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado con parámetros de salida (INOUT).
        /// Los valores de salida se leen desde el mismo DynamicParameters después de la ejecución.
        /// </summary>
        /// <param name="storedProcedure">Nombre del procedimiento almacenado.</param>
        /// <param name="parameters">
        /// DynamicParameters con los parámetros de entrada y salida configurados.
        /// Ejemplo de parámetro de salida:
        /// parameters.Add("p_id", dbType: DbType.Int32, direction: ParameterDirection.Output);
        /// </param>
        public async Task ExecuteWithOutputAsync(string storedProcedure, DynamicParameters parameters)
        {
            await using var connection = CreateConnection();
            await connection.ExecuteAsync(BuildFunctionSql(storedProcedure, parameters), parameters);
        }

        /// <summary>
        /// Ejecuta múltiples operaciones de base de datos dentro de una transacción atómica.
        /// Si alguna operación falla, todas se revierten automáticamente.
        /// </summary>
        /// <param name="operation">
        /// Función que recibe la conexión y la transacción activa.
        /// Todas las operaciones dentro deben usar la misma conexión y transacción.
        /// Ejemplo:
        /// await _db.ExecuteInTransactionAsync(async (conn, trans) => {
        ///     await conn.ExecuteAsync("sp_insert_order", orderParams, trans, commandType: CommandType.StoredProcedure);
        ///     await conn.ExecuteAsync("sp_insert_detail", detailParams, trans, commandType: CommandType.StoredProcedure);
        /// });
        /// </param>
        public async Task ExecuteInTransactionAsync(Func<IDbConnection, IDbTransaction, Task> operation)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await operation(connection, transaction);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
