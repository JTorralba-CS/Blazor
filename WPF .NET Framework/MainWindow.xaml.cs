using Microsoft.AspNetCore.SignalR.Client;
using Standard;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPF.NET_Framework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UniformResourceLocator _Server = new UniformResourceLocator();

        private UniformResourceLocator _ChatHub = new UniformResourceLocator();

        HubConnection _HubConnection;

        private bool _IsConnected => _HubConnection?.State == HubConnectionState.Connected;

        private bool IsConnected
        {
            get { return (bool)GetValue(IsConnectedProperty); }
            set { SetValue(IsConnectedProperty, value); }
        }

        public static readonly DependencyProperty IsConnectedProperty =
            DependencyProperty.Register("IsConnected", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        private Message _Message = new Message();

        private List<String> Alias = new List<String>();

        public MainWindow()
        {
            InitializeComponent();

            _Server.SetDomain("https://localhost/");
            
            _ChatHub.URL = _Server.Domain + "chathub";

            _Message.User = "Connect";
            Alias.Add(_Message.User);

            TextBox_ChatHub.DataContext = _ChatHub;
            Button_Connect.DataContext = _Message;
            TextBox_Message.DataContext = _Message;
        }

        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            InitializeSignalR(_ChatHub);
        }

        private void Button_Send_Click(object sender, RoutedEventArgs e)
        {
            Send();
        }

        private async Task InitializeSignalR(UniformResourceLocator _ChatHub)
        {
            _HubConnection = new HubConnectionBuilder()
                .WithUrl(_ChatHub.URL)
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

            if (_IsConnected)
            {
                await RefreshConnected("");
            }
        }

        private async Task RefreshConnected(String S)
        {
            this.Dispatcher.Invoke(() =>
            {
                IsConnected = true;
                _Message.User = _HubConnection.ConnectionId.ToString().ToUpper().Substring(0, 5);
                Alias.Add(_Message.User);
            });
        }

        private async Task RefreshDisconnected(Exception E)
        {
            this.Dispatcher.Invoke(() =>
            {
                IsConnected = false;
                _Message.User = "Connect";
            });
        }

        private async Task Send()
        {
            _Message.Time = DateTime.Now;

            if (_Message.Content != null)
            {
                if (_Message.Content.Trim() == "")
                {
                    return;
                }
                else
                {
                    String NameCheck = _Message.Content.Trim().ToLower();

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
                            }

                            _Message.User = NameCheckList[3].ToUpper().Trim();
                            Alias.Add(_Message.User);
                        }
                    }
                    else
                    {
                        if (_HubConnection != null)
                        {
                            await _HubConnection.SendAsync("TX", _Message);
                            _Message.Content = "";
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

    public class BoolInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
