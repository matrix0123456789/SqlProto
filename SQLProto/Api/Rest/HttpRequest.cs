using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SQLProto.Utils;

namespace SQLProto.Api.Rest
{
    public class HttpRequest
    {
        public Dictionary<string, string> Headers = new Dictionary<string, string>();
        public string Method { get; set; }
        private string path;
        private EndPoint clientLocalEndPoint;
        private string body;

        public async Task<string> GetBodyText()
        {
            if (body == null)
            {
                var bytes = new byte[long.Parse(Headers["content-length"])];
                await stream.ReadAsync(bytes, 0, bytes.Length);
                body = System.Text.UTF8Encoding.UTF8.GetString(bytes);
            }
            return body;
        }

        private Stream stream;

        public Uri Uri => new Uri("http://" +
                                  (Headers.ContainsKey("host") ? Headers["host"] : clientLocalEndPoint.ToString()) +
                                  path);
        public static async Task<HttpRequest> Parse(Stream stream, EndPoint clientLocalEndPoint)
        {
            var ret = new HttpRequest();
            var firstLine = await stream.ReadUtf8LineAsync();
            ret.ParseFirstLine(firstLine);
            while (true)
            {
                var line = stream.ReadUtf8Line();
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }

                ret.ParseHeader(line);
            }

            ret.stream = stream;
            ret.clientLocalEndPoint = clientLocalEndPoint;
            return ret;
        }

        private void ParseHeader(string line)
        {
            var semicolon = line.IndexOf(':');
            if (semicolon > 0)
            {
                var key = line.Substring(0, semicolon).Trim().ToLowerInvariant();
                var value = line.Substring(semicolon + 1).Trim();
                Headers[key] = value;
            }
        }

        private void ParseFirstLine(string firstLine)
        {
            var regex = new Regex("^ *(\\w+) +(\\S+) +(\\S+) *$");
            var groups = regex.Match(firstLine).Groups;
            Method = groups[1].Value;
            path = groups[2].Value;
        }
    }
}