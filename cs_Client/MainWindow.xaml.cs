using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cs_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Socket _client;
        private readonly IPEndPoint _remoteEp;
        private readonly byte[] _buffer;
        private readonly Socket _listener;
        private  EndPoint _endPoint;

        public MainWindow()
        {
            InitializeComponent();

            _client = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);

            var ip = IPAddress.Parse("127.0.0.1");
            const int port = 45678;
            _remoteEp = new IPEndPoint(ip, port);

             _buffer = Encoding.Default.GetBytes("Start");

             _listener = new Socket(
                 AddressFamily.InterNetwork,
                 SocketType.Dgram,
                 ProtocolType.Udp);

             const int listenerPort = 45679;
             var listenerEp = new IPEndPoint(ip, listenerPort);
             _listener.Bind(listenerEp);

             _endPoint = new IPEndPoint(IPAddress.Any, 0);

        }

        private void ButtonTakeScreenShot_Click(object sender, EventArgs e)
        {
            _client.SendTo(_buffer, _remoteEp);
            ReceivePhoto();
        }

        private void PullImage(MemoryStream ms)
        {
            var imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.CacheOption = BitmapCacheOption.OnLoad;
            imageSource.StreamSource = ms;
            imageSource.EndInit();
            image1.Source= imageSource;
        }

        private void ReceivePhoto()
        {
            var bytes = new byte[ushort.MaxValue - 28];

            var len =_listener.ReceiveFrom(bytes, ref _endPoint);
            using (var ms = new MemoryStream(bytes, 0, len))
            {
                PullImage(ms);
            }

        }
    }
}

