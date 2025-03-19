using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using VoiceMatters.Application.Services;

namespace VoiceMatters.Infrastructure.Hubs
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<VoiceMattersHub> _hubContext;

        public NotificationService(IHubContext<VoiceMattersHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PetitionCreated()
        {
            var json = JsonConvert.SerializeObject(new { Method = nameof(PetitionCreated) });
            await _hubContext.Clients.All.SendAsync(json);
        }

        public async Task PetitionDeleted(int signsQuantity)
        {
            var data = new { Method = nameof(PetitionDeleted), Quantity = signsQuantity };
            var json = JsonConvert.SerializeObject(data);
            await _hubContext.Clients.All.SendAsync(json);
        }

        public async Task PetitionSigned()
        {
            var json = JsonConvert.SerializeObject(new { Method = nameof(PetitionSigned) });
            await _hubContext.Clients.All.SendAsync(json);
        }

        public async Task UserRegistered()
        {
            var json = JsonConvert.SerializeObject(new { Method = nameof(UserRegistered) });
            await _hubContext.Clients.All.SendAsync(json);
        }
    }
}
