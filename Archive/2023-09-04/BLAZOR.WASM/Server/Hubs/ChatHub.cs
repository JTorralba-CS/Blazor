using BLAZOR.WASM.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BLAZOR.WASM.Server.Hubs;

public class ChatHub : Hub
{
    private WeatherForecastService _WeatherForecast;

    public ChatHub(WeatherForecastService WeatherForecast)
    {
        _WeatherForecast = WeatherForecast;

    }

    public WeatherForecast[] Forecasts;

    public Task TX(String User, String Message)
    {

        if (Message.ToUpper().Trim() == "~")
        {
            CallMessageX(User);
            return Task.CompletedTask;
        }

        return Clients.All.SendAsync("RX", User.ToUpper(), Message);
    }

    private async void CallMessageX(String User)
    {
        String Message;
        Message = await TaskMessageX();
        Clients.All.SendAsync("RX", User, Message);
    }

    protected async Task<String> TaskMessageX()
    {
        String Message = "";

        Forecasts = await _WeatherForecast.GetForecastAsync(DateTime.Now);

        foreach (var Forcast in Forecasts)
        {
            Message = String.Concat(Message, " ", Forcast.TemperatureF.ToString().Trim());
        }

        return Message;
    }
}
