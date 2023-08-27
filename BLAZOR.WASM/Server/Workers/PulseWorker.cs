using Microsoft.AspNetCore.SignalR;
using BLAZOR.WASM.Server.Hubs;

namespace BLAZOR.WASM.Server.Workers
{
    public class PulseWorker : BackgroundService
    {
        private IHubContext<ChatHub> _ChatHubContext;

        public PulseWorker(IHubContext<ChatHub> ChatHubContext)
        {
            _ChatHubContext = ChatHubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(17000);

                await _ChatHubContext.Clients.All.SendAsync("RX", "Worker", "<PULSE>");
            }
        }
    }
}
