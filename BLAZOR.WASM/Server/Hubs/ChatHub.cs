using Microsoft.AspNetCore.SignalR;

namespace BLAZOR.WASM.Server.Hubs;

public class ChatHub : Hub
{
    public Task TX(String User, string Message)
    {
        return Clients.All.SendAsync("RX", User, Message);
    }
}
