using System;
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
            listener.Start();
            thread = new Thread(MainLoop);
            thread.Start();
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
            try
            {
                var request = await HttpRequest.Parse(stream, connection.Client.LocalEndPoint);
                var controller = new Controller();
                var result = await controller.Invoke(request);
                result.Send(stream);
            }catch(Exception ex)
            {
                var result = new HttpResponse(System.Net.HttpStatusCode.InternalServerError, ex.ToString());
                result.Send(stream);

            }
            finally
            {
                stream.Close();
            }
        }
    }
}