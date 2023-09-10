using Microsoft.AspNetCore.SignalR.Client;
using System;
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
            //InizializzaConnectioTuBlazorHub();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Send();
        }

        async Task InizializzaConnectioTuBlazorHub(String URL)
        {
            _HubConnection = new HubConnectionBuilder()
                .WithUrl(URL)
                .WithAutomaticReconnect()
                .Build();

            _HubConnection.Closed += RefreshDisconnected;
            _HubConnection.Reconnecting += RefreshDisconnected;
            _HubConnection.Reconnected += RefreshConnected;

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
                            ListBox_Messages.Items.Add($"{User}: {Message}");
                            UpdateScrollBar(ListBox_Messages);
                        }
                    });

            });

            await _HubConnection.StartAsync();

            if (IsConnected)
            {
                TextBox_URL.IsEnabled = !IsConnected;
                Button_Connect.IsEnabled = !IsConnected;
                TextBox_Message.IsEnabled = IsConnected;
                Button_Send.IsEnabled = IsConnected;
                User = _HubConnection.ConnectionId.ToString().ToUpper().Substring(0, 5);
            }
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
            ListBox_Messages.Items.Add($"{User}: {Message}");
            UpdateScrollBar(ListBox_Messages);
        }

        protected String TaskMessageX()
        {
            String Message = "`````````````````";
            return Message;
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

        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            InizializzaConnectioTuBlazorHub(TextBox_URL.Text);
        }

        async Task RefreshDisconnected(Exception E)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (!IsConnected)
                {
                    TextBox_URL.IsEnabled = !IsConnected;
                    Button_Connect.IsEnabled = !IsConnected;
                    TextBox_Message.IsEnabled = IsConnected;
                    Button_Send.IsEnabled = IsConnected;
                }
            });
        }

        async Task RefreshConnected(String S)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (IsConnected)
                {
                    TextBox_URL.IsEnabled = !IsConnected;
                    Button_Connect.IsEnabled = !IsConnected;
                    TextBox_Message.IsEnabled = IsConnected;
                    Button_Send.IsEnabled = IsConnected;
                    User = _HubConnection.ConnectionId.ToString().ToUpper().Substring(0, 5);
                }
            });
        }

    }
}