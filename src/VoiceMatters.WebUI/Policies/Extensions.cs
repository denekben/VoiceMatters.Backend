using Microsoft.AspNetCore.Authorization;

namespace VoiceMatters.WebUI.Policies
{
    public static class Extensions
    {
        public static IServiceCollection AddPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsNotBlocked", policy => policy.Requirements.Add(new IsNotBlockedRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, IsNotBlockedHandler>();

            return services;
        }
    }
}
