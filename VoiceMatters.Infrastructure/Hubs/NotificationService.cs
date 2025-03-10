using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Application.Services;

namespace VoiceMatters.Infrastructure.Hubs
{
    internal class NotificationService : INotificationService
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

        public async Task PetitionDeleted()
        {
            await _hubContext.Clients.All.SendAsync(nameof(PetitionDeleted));
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
