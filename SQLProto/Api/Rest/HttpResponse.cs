using System.Net;

namespace SQLProto.Api.Rest
{
    public class HttpResponse
    {
        public HttpResponse(HttpStatusCode code)
        {
            Code = code;
        }

        public HttpStatusCode Code { get; set; }
    }
}