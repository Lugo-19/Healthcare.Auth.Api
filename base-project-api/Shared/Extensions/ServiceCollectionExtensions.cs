using Healthcare.Auth.Api.Core.Auth.Interfaces;
using Healthcare.Auth.Api.Core.Auth.Repositories;
using Healthcare.Auth.Api.Core.Auth.Services;
using Healthcare.Auth.Api.Core.Permisos.Interfaces;
using Healthcare.Auth.Api.Core.Permisos.Repositories;
using Healthcare.Auth.Api.Core.Permisos.Services;
using Healthcare.Auth.Api.Core.Roles.Interfaces;
using Healthcare.Auth.Api.Core.Roles.Repositories;
using Healthcare.Auth.Api.Core.Roles.Services;
using Healthcare.Auth.Api.Core.Usuarios.Interfaces;
using Healthcare.Auth.Api.Core.Usuarios.Repositories;
using Healthcare.Auth.Api.Core.Usuarios.Services;
using Healthcare.Auth.Api.Shared.Helpers;
using FluentValidation;
using System.Reflection;

namespace Healthcare.Auth.Api.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddScoped<DbExecutor>();
            services.AddScoped<HttpExecutor>();
            services.AddScoped<JwtHelper>();

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IRolRepository, RolRepository>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IPermisoRepository, PermisoRepository>();
            services.AddScoped<IPermisoService, PermisoService>();

            // Registra automáticamente todos los Profile que hereden de AutoMapper.Profile
            services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

            // Registra automáticamente todos los validators que hereden de AbstractValidator<T>
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
