using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using VoiceMatters.Application.Services;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Infrastructure.BackgroundServices;
using VoiceMatters.Infrastructure.Data;
using VoiceMatters.Infrastructure.Hubs;
using VoiceMatters.Infrastructure.Repositories;
using VoiceMatters.Infrastructure.Services;

namespace VoiceMatters.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IImageService, ImageService>();

            services.AddScoped<IAppUserPetitionRepository, AppUserPetitionRepository>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<IPetitionRepository, PetitionRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IStatisticRepository, StatisticRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IPetitionTagRepository, PetitionTagRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IRepository, Repository>();

            services.AddSignalR();
            services.AddScoped<INotificationService, NotificationService>();

            services.AddHttpClient();

            services.AddHostedService<DailySignsRefreshService>();

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddDbContext<AppDbContext>(
                options => options.UseNpgsql(configuration["ConnectionString:DefaultConntection"])
            );
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                options.DefaultChallengeScheme =
                options.DefaultForbidScheme =
                options.DefaultScheme =
                options.DefaultSignInScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(configuration["JWT:SigningKey"])
                    )
                };
            });

            return services;
        }

        public static WebApplication UseInfrastructure(this WebApplication app)
        {
            app.MapHub<VoiceMattersHub>("voice-matters-hub");

            return app;
        }
    }
}
