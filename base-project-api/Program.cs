using Asp.Versioning;
using Healthcare.Auth.Api.Shared.Extensions;
using Healthcare.Auth.Api.Shared.Middlewares;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

// Mantener compatibilidad: DateTime mapea a 'timestamp without time zone' en lugar de 'timestamptz'
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Mapear columnas snake_case de PostgreSQL a propiedades PascalCase de C#
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();

builder.Services.AddControllers();

builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddHttpClient();

builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAll", b =>
    {
        b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddHealthChecks(builder.Configuration);

builder.Services.AddSharedServices();

builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var swaggerEndPoint = builder.Configuration.GetValue<string>("SwaggerEndPoint");
    var projectName = builder.Configuration.GetValue<string>("ProjectName");

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            c.SwaggerEndpoint($"{swaggerEndPoint}/swagger/{description.GroupName}/swagger.json",$"{projectName} {description.GroupName.ToUpper()}");
        }
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.MapControllers();
app.MapHealthChecks();

app.Run();
