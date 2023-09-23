using Microsoft.AspNetCore.SignalR.Client;
using Standard;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
        HubConnection _HubConnection;
        private bool IsConnected => _HubConnection?.State == HubConnectionState.Connected;

        private Message _Message = new Message();
        private List<String> Alias = new List<String>();

        public MainWindow()
        {
            InitializeComponent();
            _Message.User = "Connect";
            Alias.Add(_Message.User);
        }

        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            InitializeSignalR(TextBox_ChatHub.Text);
        }

        private void Button_Send_Click(object sender, RoutedEventArgs e)
        {
            Send();
        }

        private async Task InitializeSignalR(String ChatHub)
        {
            _HubConnection = new HubConnectionBuilder()
                .WithUrl(ChatHub)
                .WithAutomaticReconnect()
                .Build();

            _HubConnection.Closed += RefreshDisconnected;
            _HubConnection.Reconnecting += RefreshDisconnected;
            _HubConnection.Reconnected += RefreshConnected;

            _HubConnection.On<Message>("RX", (_Message) =>
            {
                this.Dispatcher.Invoke(() =>
                    {
                        if (_Message.Content.ToUpper().Trim() == "`")
                        {
                            CustomClientEvent(_Message);
                        }
                        else if (_Message.Content.Contains(Convert.ToChar(30).ToString()))
                        {
                            ParseTable(_Message);
                        }
                        else
                        {
                            AppendMessage(_Message);
                        }
                    });
            });

            await _HubConnection.StartAsync();

            if (IsConnected)
            {
                RefreshConnected("");
            }
        }

        private async Task RefreshConnected(String S)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (IsConnected)
                {
                    TextBox_ChatHub.IsEnabled = !IsConnected;
                    Button_Connect.IsEnabled = !IsConnected;
                    TextBox_Message.IsEnabled = IsConnected;
                    Button_Send.IsEnabled = IsConnected;
                    _Message.User = _HubConnection.ConnectionId.ToString().ToUpper().Substring(0, 5);
                    Button_Connect.Content = _Message.User;
                    Alias.Add(_Message.User);
                }
            });
        }

        private async Task RefreshDisconnected(Exception E)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (!IsConnected)
                {
                    TextBox_ChatHub.IsEnabled = !IsConnected;
                    Button_Connect.IsEnabled = !IsConnected;
                    TextBox_Message.IsEnabled = IsConnected;
                    Button_Send.IsEnabled = IsConnected;
                    _Message.User = "Connect";
                    Button_Connect.Content = _Message.User;
                }
            });
        }

        private async Task Send()
        {
            _Message.Time = DateTime.Now;

            if (TextBox_Message.Text != null)
            {
                if (TextBox_Message.Text.Trim() == "")
                {
                    return;
                }
                else
                {
                    String NameCheck = TextBox_Message.Text.Trim().ToLower();

                    if (NameCheck.StartsWith("mi nombre es"))
                    {
                        Char[] Space = " ".ToCharArray();
                        String[] NameCheckList = NameCheck.Split(Space);
                        if (NameCheckList.Length == 4)
                        {
                            if (_HubConnection != null)
                            {
                                _Message.Content = "Call me " + NameCheckList[3].ToUpper() + ".";
                                await _HubConnection.SendAsync("TX", _Message);
                                _Message.Content = "";
                                TextBox_Message.Text = "";
                            }

                            _Message.User = NameCheckList[3].ToUpper().Trim();
                            Button_Connect.Content = _Message.User;
                            Alias.Add(_Message.User);
                        }
                    }
                    else
                    {
                        if (_HubConnection != null)
                        {
                            _Message.Content = TextBox_Message.Text;
                            await _HubConnection.SendAsync("TX", _Message);
                            _Message.Content = "";
                            TextBox_Message.Text = "";
                        }
                    }
                }
            }
        }

        private void CustomClientEvent(Message _Message)
        {
            _Message.Content = "<Custom Client Event>";
            AppendMessage(_Message);
        }

        private void AppendMessage(Message _Message)
        {
            _Message.Content = _Message.Content.Trim();

            BrushConverter BC = new BrushConverter();
            String Color;

            if (_Message.User == this._Message.User || Alias.Contains(_Message.User))
            {
                Color = "Green";
            }
            else if (_Message.Content == "\u2764")
            {
                Color = "Red";
            }
            else
            {
                Color = "Blue";
            }

            TextRange TR_User = new TextRange(RichTextBox.Document.ContentEnd, RichTextBox.Document.ContentEnd);
            TR_User.Text = _Message.Time.ToString("yyyy-MM-dd_HH:mm:ss.fff") + " " + _Message.User + ": ";
            try
            {
                TR_User.ApplyPropertyValue(TextElement.ForegroundProperty, BC.ConvertFromString("Black"));
            }
            catch (FormatException)
            {
            }

            TextRange TR_Message = new TextRange(RichTextBox.Document.ContentEnd, RichTextBox.Document.ContentEnd);
            TR_Message.Text = _Message.Content;
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

        private void ParseTable(Message _Message)
        {
            String[] Rows = _Message.Content.Split(Convert.ToChar(30));
            foreach (String Row in Rows)
            {
                if (Row != "")
                {
                    Message _Record = new Message();
                    _Record.Time = _Message.Time;
                    _Record.User = _Message.User;
                    _Record.Content = "";

                    String[] Columns = Row.Split(Convert.ToChar(31));
                    foreach (String Column in Columns)
                    {
                        if (Column != "")
                        {
                            _Record.Content = _Record.Content + " " + Column;
                        }
                    }
                    AppendMessage(_Record);
                }
            }
        }
    }
}
