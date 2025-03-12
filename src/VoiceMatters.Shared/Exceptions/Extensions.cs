using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Exceptions
{
    public static class Extensions
    {
        public static IServiceCollection AddErrorHandling(this IServiceCollection services)
            => services
                .AddScoped<ErrorHandlerMiddleware>()
                .AddSingleton<IExceptionToResponseMapper, ExceptionToResponseMapper>();
        
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ErrorHandlerMiddleware>();
    }
    }
