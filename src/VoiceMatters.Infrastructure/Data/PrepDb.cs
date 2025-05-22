using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Infrastructure.Data
{
    public static class PrepDb
    {
        public static async Task ApplyMigrationsAsync<TContext>(IServiceProvider services)
                    where TContext : DbContext
        {
            using var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetService<ILogger<TContext>>();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();

            try
            {
                logger?.LogInformation("Checking for pending migrations for {DbContext}...", typeof(TContext).Name);

                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

                if (!pendingMigrations.Any())
                {
                    logger?.LogInformation("No pending migrations found");
                    return;
                }

                logger?.LogInformation("Found {Count} migrations to apply: {Migrations}",
                    pendingMigrations.Count(), string.Join(", ", pendingMigrations));

                await context.Database.MigrateAsync();

                logger?.LogInformation("All migrations applied successfully");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to apply migrations");
                throw new BadRequestException($"Migration failed: {ex.Message}");
            }
        }

        public static async Task MigrateToAsync<TContext>(
            IServiceProvider services,
            string targetMigration)
            where TContext : DbContext
        {
            using var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetService<ILogger<TContext>>();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();

            try
            {
                logger?.LogInformation("Applying migrations up to {TargetMigration}...", targetMigration);
                await context.Database.MigrateAsync();
                logger?.LogInformation("Successfully migrated to {TargetMigration}", targetMigration);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to migrate to {TargetMigration}", targetMigration);
                throw new BadRequestException($"Migration to {targetMigration} failed: {ex.Message}");
            }
        }

        public static async Task<bool> HasPendingMigrationsAsync<TContext>(IServiceProvider services)
            where TContext : DbContext
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            return pendingMigrations.Any();
        }
    }
}
