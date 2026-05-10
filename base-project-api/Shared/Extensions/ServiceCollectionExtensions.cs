using Healthcare.Auth.Api.Core.Auth.Interfaces;
using Healthcare.Auth.Api.Core.Auth.Repositories;
using Healthcare.Auth.Api.Core.Auth.Services;
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

            // Registra automáticamente todos los Profile que hereden de AutoMapper.Profile
            services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

            // Registra automáticamente todos los validators que hereden de AbstractValidator<T>
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
