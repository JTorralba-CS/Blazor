using Microsoft.AspNetCore.SignalR;
using Standard;

namespace WASM.Server.Hubs;

public class ChatHub : Hub
{
    public Task TX(Message _Message)
    {
        if (_Message.Content.ToUpper().Trim() == "~")
        {
            CustomServerEvent(_Message);
            return Task.CompletedTask;
        }
        else
        {
            return Clients.All.SendAsync("RX", _Message);
        }
    }

    private async void CustomServerEvent(Message _Message)
    {
        _Message.Content = "<Custom Server Event>";
        Clients.All.SendAsync("RX", _Message);
    }
}
