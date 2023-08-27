using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WPF.NET_Framework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection _HubConnection;

        public MainWindow()
        {
            InitializeComponent();
            InizializzaConnectioTuBlazorHub();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Send();
        }

        async Task InizializzaConnectioTuBlazorHub()
        {
            _HubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost/chathub")
                .WithAutomaticReconnect()
                .Build();

            _HubConnection.On<String, String>("RX", (User, Message) =>
            {
                
                    this.Dispatcher.Invoke(() =>
                        {
                            if (Message.ToUpper().Trim() == "`")
                            {
                                CallMessageX(User);
                            }
                            else
                            {
                                ListBox_Message.Items.Add($"{User}: {Message}");
                            }
                        });
                
            });

            await _HubConnection.StartAsync();
            Button_Send.IsEnabled = IsConnected;
        }

        public bool IsConnected => _HubConnection?.State == HubConnectionState.Connected;

        async Task Send()
        {
            if (_HubConnection != null)
            {
                await _HubConnection.SendAsync("TX", TextBox_User.Text , TextBox_Message.Text);
            }
        }

        private void CallMessageX(String User)
        {
            String Message = "";
            Message = TaskMessageX();
            ListBox_Message.Items.Add($"{User}: {Message}");
        }

        protected String TaskMessageX()
        {
            String Message = "";

            Forecasts = GetForecastAsync(DateTime.Now);

            foreach (var Forcast in Forecasts)
            {
                Message = String.Concat(Message, " ", Forcast.Date.ToString().Trim());
            }

            return Message;
        }

        public class WeatherForecast
        {
            public DateTime Date { get; set; }

            public int TemperatureC { get; set; }

            public string Summary { get; set; }

            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        }

        private WeatherForecast[] Forecasts;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecast[] GetForecastAsync(DateTime startDate)
        {
            return Enumerable.Range(1, 17).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = new Random().Next(-20, 55),
                Summary = Summaries[new Random().Next(Summaries.Length)]
            }).ToArray();
        }
    }
}
