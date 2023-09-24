using Microsoft.AspNetCore.SignalR;
using Standard;

namespace WASM.Server.Hubs;

public class ChatHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Message _Message = new Message();

        _Message.Time = DateTime.Now;
        _Message.User = "SIGNALR";
        _Message.Content = Context.ConnectionId.ToString().ToUpper().Substring(0, 5) + " connected.";

        Clients.All.SendAsync("RX", _Message);

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Message _Message = new Message();

        _Message.Time = DateTime.Now;
        _Message.User = "SIGNALR";
        _Message.Content = Context.ConnectionId.ToString().ToUpper().Substring(0, 5) + " disconnected.";

        Clients.All.SendAsync("RX", _Message);

        return base.OnDisconnectedAsync(exception);
    }

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
