using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Security.Policy;
using System.Runtime.Remoting.Messaging;
using System.Windows.Controls;
using System.Diagnostics;

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
                    ListBox_Message.Items.Add($"{User}: {Message}");
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
    }
}
