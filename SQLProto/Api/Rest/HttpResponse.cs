using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SQLProto.Api.Rest
{
    public class HttpResponse
    {
        public HttpResponse(HttpStatusCode code, string body = null)
        {
            Code = code;
            Body = body;
        }

        public HttpStatusCode Code { get; set; }
        public string Body { get; set; }

        internal void Send(NetworkStream stream)
        {
            var headers = new List<string> { "HTTP/1.1 " + (int)Code + " " + Code.ToString() };
            foreach (var header in headers)
            {
                stream.Write(UTF8Encoding.UTF8.GetBytes(header));
                stream.Write(UTF8Encoding.UTF8.GetBytes("\r\n"));
            }
            if (Body != null)
            {
                stream.Write(UTF8Encoding.UTF8.GetBytes("\r\n"));
                stream.Write(UTF8Encoding.UTF8.GetBytes(Body));
            }
        }
    }
}