using Microsoft.AspNetCore.SignalR;
using BLAZOR.WASM.Server.Hubs;

namespace BLAZOR.WASM.Server.Workers
{
    public class HEARTBEAT : BackgroundService
    {
        private IHubContext<ChatHub> _ChatHubContext;

        public HEARTBEAT(IHubContext<ChatHub> ChatHubContext)
        {
            _ChatHubContext = ChatHubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(16 * 1000);

                await _ChatHubContext.Clients.All.SendAsync("RX", "SIGNALR", "\u2764");
            }
        }
    }
}
