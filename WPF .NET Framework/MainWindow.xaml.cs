using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPF.NET_Framework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String User = "Connect";

        HubConnection _HubConnection;

        public bool IsConnected => _HubConnection?.State == HubConnectionState.Connected;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            InitializeSignalR(TextBox_URL.Text);
        }

        private void Button_Send_Click(object sender, RoutedEventArgs e)
        {
            Send();
        }

        async Task InitializeSignalR(String URL)
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
                            CustomClientEvent(User);
                        }
                        else
                        {
                            AppendMessage(User, Message);
                        }
                    });
            });

            await _HubConnection.StartAsync();

            if (IsConnected)
            {
                RefreshConnected("");
            }
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
                    Button_Connect.Content = User;
                }
            });
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
                    User = "Connect";
                    Button_Connect.Content = User;
                }
            });
        }

        async Task Send()
        {
            if (_HubConnection != null)
            {
                await _HubConnection.SendAsync("TX", User, TextBox_Message.Text);
                TextBox_Message.Text = "";
            }
        }

        private void CustomClientEvent(String User)
        {
            AppendMessage(User, "<Custom Client Event>");
        }

        public void AppendMessage(String User, String Message)
        {
            BrushConverter BC = new BrushConverter();
            String Color;

            if (User == this.User)
            {
                Color = "Green";
            }
            else if (User == "SIGNALR")
            {
                Color = "Red";
            }
            else
            {
                Color = "Blue";
            }

            TextRange TR_User = new TextRange(RichTextBox.Document.ContentEnd, RichTextBox.Document.ContentEnd);
            TR_User.Text = User + ":  ";
            try
            {
                TR_User.ApplyPropertyValue(TextElement.ForegroundProperty, BC.ConvertFromString("Black"));
            }
            catch (FormatException)
            {
            }

            TextRange TR_Message = new TextRange(RichTextBox.Document.ContentEnd, RichTextBox.Document.ContentEnd);
            TR_Message.Text = Message;
            try
            {
                TR_Message.ApplyPropertyValue(TextElement.ForegroundProperty, BC.ConvertFromString(Color));
            }
            catch (FormatException)
            {
            }

            TextRange TR_CR = new TextRange(RichTextBox.Document.ContentEnd, RichTextBox.Document.ContentEnd);
            TR_CR.Text = "\r";
            try
            {
                TR_CR.ApplyPropertyValue(TextElement.ForegroundProperty, BC.ConvertFromString("Black"));
            }
            catch (FormatException)
            {
            }

            RichTextBox.ScrollToEnd();
        }
    }
}
