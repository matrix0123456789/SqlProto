using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SQLProto.Api.Rest;
using Xunit;

namespace SqlProto.Tests.Api.Rest
{
    public class HttpRequestTest
    {
        const string example1 = @"GET /wp-includes/css/dist/block-library/theme.min.css?ver=5.4.2 HTTP/1.1
Host: example.io
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
User-Agent: Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Mobile Safari/537.36
Accept: text/css,*/*;q=0.1
Sec-Fetch-Site: same-origin
Sec-Fetch-Mode: no-cors
Sec-Fetch-Dest: style
Referer: https://ecologic.io/pl/
Accept-Encoding: gzip, deflate, br
Referer: https://ecologic.io/pl/
Accept-Language: pl,eo;q=0.9,en-US;q=0.8,en;q=0.7
Cookie: pll_language=pl
";

        private const string example2 = @"GET / HTTP/1.0
";

        [Fact]
        public async Task Method()
        {
            var obj = await Prepare(example1);
            Assert.Equal("GET", obj.Method);
        }

        [Fact]
        public async Task Uri()
        {
            var obj = await Prepare(example1);
            Assert.Equal(new Uri("http://example.io/wp-includes/css/dist/block-library/theme.min.css?ver=5.4.2"),
                obj.Uri);
        }
        
        [Fact]
        public async Task Header()
        {
            var obj = await Prepare(example1);
            Assert.Equal("no-cache", obj.Headers["cache-control"]);
            Assert.Equal("Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Mobile Safari/537.36", obj.Headers["user-agent"]);
        }

        private async Task<HttpRequest> Prepare(string rawRequest)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(rawRequest));
            return await HttpRequest.Parse(stream, IPEndPoint.Parse("127.0.0.1:80"));
        }
    }
}