using VoiceMatters.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Shared.Exceptions;
using Microsoft.AspNetCore.Builder;

namespace VoiceMatters.Shared
{
    public static class Extensions
    {
        public static IServiceCollection AddShared(this IServiceCollection services)
        {
            services.AddScoped<IHttpContextService, HttpContextService>();
            services.AddHttpContextAccessor();
            services.AddErrorHandling();


            return services;
        }

        public static IApplicationBuilder UseShared(this IApplicationBuilder app)
        {
            app.UseErrorHandling();

            return app;
        }
    }
}
