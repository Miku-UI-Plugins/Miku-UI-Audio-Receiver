using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Miku_UI_Audio_Receiver.Windows
{
    /// <summary>
    /// Interaction logic for LyricFloatWindow.xaml
    /// </summary>
    public partial class LyricFloatWindow : Window
    {
        public LyricFloatWindow()
        {
            InitializeComponent();
            InitListener();
        }

        private async void InitListener()
        {
            // This is the IP address of the local machine
            IPAddress localIpAddress = IPAddress.Parse(Utils.IPTools.GetLocalIPv4());
            IPEndPoint ipEndPoint = new(localIpAddress, 39393);

            using Socket listener = new(
                ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            listener.Bind(ipEndPoint);
            listener.Listen(100);
            LyricText.Text = $"Listening on {localIpAddress}:{ipEndPoint.Port}...";
            await StartListen(listener);
        }

        private async Task StartListen(Socket socket)
        {
            var handler = await socket.AcceptAsync();
            while (true)
            {
                // Receive lyric.
                var buffer = new byte[1_024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);

                var eom = "<|EOM|>";
                if (response.IndexOf(eom) > -1 /* is end of Lyric */)
                {
                    LyricText.Text = response.Replace(eom, "");
                    await StartListen(socket);
                    break;
                }
            }
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
