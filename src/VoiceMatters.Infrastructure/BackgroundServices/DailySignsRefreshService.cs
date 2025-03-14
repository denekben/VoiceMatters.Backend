using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VoiceMatters.Infrastructure.Data;

namespace VoiceMatters.Infrastructure.BackgroundServices
{
    class DailySignsRefreshService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;

        public DailySignsRefreshService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoRefresh, null, TimeSpan.Zero, TimeSpan.FromHours(6));
            return Task.CompletedTask;
        }

        private async void DoRefresh(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                var today = DateTime.Today;

                var petitions = await context.Petitions
                    .Include(p => p.SignedUsers)
                    .ToListAsync();

                foreach (var petition in petitions)
                {
                    var signedToday = petition.SignedUsers
                        .Count(s => s.SignedDate.Date == today);

                    petition.SignQuantityPerDay = (uint)signedToday;

                    context.Entry(petition).Property(p => p.SignQuantityPerDay).IsModified = true;
                }

                await context.SaveChangesAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
