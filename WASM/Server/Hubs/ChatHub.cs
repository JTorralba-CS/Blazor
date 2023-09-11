using Microsoft.AspNetCore.SignalR;

namespace WASM.Server.Hubs;

public class ChatHub : Hub
{
    public Task TX(String User, String Message)
    {
        if (Message.ToUpper().Trim() == "~")
        {
            CustomServerEvent(User);
            return Task.CompletedTask;
        }
        else
        {
            return Clients.All.SendAsync("RX", User, Message);
        }
    }

    private async void CustomServerEvent(String User)
    {
        Clients.All.SendAsync("RX", User, "<Custom Server Event>");
    }
}
