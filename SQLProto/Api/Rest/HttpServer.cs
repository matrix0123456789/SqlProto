using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SQLProto.Api.Rest
{
    public class HttpServer
    {
        private TcpListener listener;
        private Thread thread;

        public HttpServer(ushort port)
        {
            listener = new TcpListener(port);
            thread = new Thread(MainLoop);
        }

        private void MainLoop()
        {
            while (true)
            {
                var connection = listener.AcceptTcpClient();
                Task.Run(() => HandleConnection(connection));
            }
        }

        private async void HandleConnection(TcpClient connection)
        {
            var stream = connection.GetStream();
            var request  =await HttpRequest.Parse(stream, connection.Client.LocalEndPoint);
            
        }
    }
}