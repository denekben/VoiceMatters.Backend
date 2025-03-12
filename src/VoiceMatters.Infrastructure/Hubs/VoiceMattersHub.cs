using Microsoft.AspNetCore.SignalR;

namespace VoiceMatters.Infrastructure.Hubs
{
    public sealed class VoiceMattersHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
