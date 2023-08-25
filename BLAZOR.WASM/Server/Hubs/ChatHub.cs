using Microsoft.AspNetCore.SignalR;

namespace BLAZOR.WASM.Server.Hubs;

public class ChatHub : Hub
{
    public Task TX(string User, string Message)
    {
        return Clients.All.SendAsync("RX", User, Message);
    }
}
