using Microsoft.AspNetCore.SignalR;
using WASM.Server.Hubs;
using Standard;

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
                Message _Message = new();

                _Message.User = "SIGNALR";
                _Message.Content = "\u2764";

                await Task.Delay(60 * 1000);

                _Message.Time = DateTime.Now;

                await _ChatHubContext.Clients.All.SendAsync("RX", _Message);
            }
        }
    }
}
