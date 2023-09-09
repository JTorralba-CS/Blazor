using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF.NET_Framework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String User = "";

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
                            UpdateScrollBar(ListBox_Message);
                        }
                    });

            });

            await _HubConnection.StartAsync();

            if (IsConnected)
            {
                User = _HubConnection.ConnectionId.ToString().ToUpper().Substring(0, 5);
            }

            Button_Send.IsEnabled = IsConnected;
        }

        public bool IsConnected => _HubConnection?.State == HubConnectionState.Connected;

        async Task Send()
        {
            if (_HubConnection != null)
            {
                await _HubConnection.SendAsync("TX", User, TextBox_Message.Text);
                TextBox_Message.Text = "";
            }
        }

        private void CallMessageX(String User)
        {
            String Message = "";
            Message = TaskMessageX();
            ListBox_Message.Items.Add($"{User}: {Message}");
            UpdateScrollBar(ListBox_Message);
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

        private void UpdateScrollBar(ListBox listBox)
        {
            if (listBox != null)
            {
                var border = (Border)VisualTreeHelper.GetChild(listBox, 0);
                var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }

        }
    }
}