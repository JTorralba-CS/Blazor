using Microsoft.AspNetCore.SignalR;

namespace WASM.Server.Hubs;

public class ChatHub : Hub
{
    public Task TX(String User, String Message)
    {
        if (Message.ToUpper().Trim() == "~")
        {
            CallMessageX(User);
            return Task.CompletedTask;
        }

        return Clients.All.SendAsync("RX", User, Message);
    }

    private async void CallMessageX(String User)
    {
        String Message;
        Message = await TaskMessageX();
        Clients.All.SendAsync("RX", User, Message);
    }

    protected async Task<String> TaskMessageX()
    {
        String Message = "~~~~~~~~~~~~~~~~~";
        return Message;
    }
}
