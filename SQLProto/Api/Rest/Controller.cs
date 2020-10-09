using System.Net;
using System.Threading.Tasks;

namespace SQLProto.Api.Rest
{
    public class Controller
    {
        public Task<HttpResponse> Invoke(HttpRequest request)
        {
            if (request.Uri.PathAndQuery == "/query" && request.Method == "POST")
            {
                
            }
            else
            {
                return new HttpResponse(HttpStatusCode.NotFound);
            }
        }
    }
}