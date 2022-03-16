using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace cs_ConsoleServer
{
    internal static class Program
    {
        private static Socket _client;
        private static IPEndPoint _connectEp;

        private static void Main()
        {

            #region Listener

            var listener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);


            var ip = IPAddress.Parse("127.0.0.1");
            const int port = 45678;

            var endPoint = new IPEndPoint(ip, port);

            listener.Bind(endPoint);

            #endregion


            #region Client

            _client = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);

            _connectEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 45679);

            #endregion

            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            var buffer= new byte[650000];

            while (true)
            {
                var len = listener.ReceiveFrom(buffer, ref remoteEndPoint);
                var message = Encoding.Default.GetString(buffer, 0, len);
                if (message != "Start") continue;
                Console.WriteLine("Message Received!");
                SendImage();
            }
        }

        private static Image ResizeImage(Image image) => new Bitmap(image, new Size(image.Width/4, image.Height/4));

        private static void SendImage()
        {
            var image = TakeScreenShot();
            var imageBytes = ConvertToByteArray(ResizeImage(image));

            _client.SendTo(imageBytes,_connectEp);

        }
        private static Image TakeScreenShot()
        {
            Console.WriteLine("Screen captured");
            return PrintScreen.CaptureScreen();
        }

        private static byte[] ConvertToByteArray(Image image)
        {
            var imageConverter = new ImageConverter();
            return (byte[])imageConverter.ConvertTo(image, typeof(byte[]))!;
        }

    }
}