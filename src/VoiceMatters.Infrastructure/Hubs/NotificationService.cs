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
            await _hubContext.Clients.All.SendAsync(nameof(PetitionCreated));
        }

        public async Task PetitionDeleted(int signsQuantity)
        {
            var data = new { Method = nameof(PetitionDeleted), Quantity = signsQuantity };
            var json = JsonConvert.SerializeObject(data);
            await _hubContext.Clients.All.SendAsync(json);
        }

        public async Task PetitionSigned()
        {
            await _hubContext.Clients.All.SendAsync(nameof(PetitionSigned));
        }

        public async Task UserRegistered()
        {
            await _hubContext.Clients.All.SendAsync(nameof(UserRegistered));
        }
    }
}
