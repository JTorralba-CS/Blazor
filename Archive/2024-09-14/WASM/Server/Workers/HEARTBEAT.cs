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

                //_Message.Content = "F1" + Convert.ToChar(31) + "F2" + Convert.ToChar(31) + "F3" + Convert.ToChar(31) + "F4" + Convert.ToChar(31) + "F5" + Convert.ToChar(30);
                //_Message.Content = _Message.Content + "S1" + Convert.ToChar(31) + "S2" + Convert.ToChar(31) + "S3" + Convert.ToChar(31) + "S4" + Convert.ToChar(31) + "S5" + Convert.ToChar(30);
                //_Message.Content = _Message.Content + "T1" + Convert.ToChar(31) + "T2" + Convert.ToChar(31) + "T3" + Convert.ToChar(31) + "T4" + Convert.ToChar(31) + "T5" + Convert.ToChar(30);
                //await _ChatHubContext.Clients.All.SendAsync("RX", _Message);
            }
        }
    }
}
