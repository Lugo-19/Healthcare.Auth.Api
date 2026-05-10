using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Healthcare.Auth.Api.Shared.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddHealthChecks().AddNpgSql(connectionString, name: "postgresql", tags: ["db"]);

            return services;
        }

        public static IEndpointRouteBuilder MapHealthChecks(this IEndpointRouteBuilder app)
        {
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = check => !check.Tags.Contains("db"),
                ResponseWriter = WriteHealthResponse
            });

            app.MapHealthChecks("/health/db", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("db"),
                ResponseWriter = WriteHealthResponse
            });

            return app;
        }

        private static async Task WriteHealthResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = report.Status.ToString(),
                duration = report.TotalDuration.TotalMilliseconds + "ms",
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    duration = e.Value.Duration.TotalMilliseconds + "ms",
                    description = e.Value.Description
                })
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
