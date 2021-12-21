using SQLProto.Schema;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SQLProto.Api.Rest
{
    public class Controller
    {
        public async Task<HttpResponse> Invoke(HttpRequest request)
        {
            var pathSplitted = request.Uri.PathAndQuery.Split('/');
            if (request.Uri.PathAndQuery == "/query" && request.Method == "POST")
            {
                var context = new Context();
                context.DefaultDB = "test";
                var (resultColumns, resultData) = context.ExecuteQuery(await request.GetBodyText()).Result;
                var result = new { columns = resultColumns, data = resultData };
                return new HttpResponse(HttpStatusCode.OK, System.Text.Json.JsonSerializer.Serialize(result));

            }
            else if (request.Uri.PathAndQuery == "/databases" && request.Method == "GET")
            {
                var result = Database.AllDatabases.Values.Select(x => new { x.Name, url = "/databases/" + x.Name, Tables = x.Tables.Values.Select(t => new { t.Name, url = "/databases/" + x.Name + "/" + t.Name, }) });
                return new HttpResponse(HttpStatusCode.OK, System.Text.Json.JsonSerializer.Serialize(result));

            }
            else if (request.Uri.PathAndQuery.StartsWith("/databases/") && request.Method == "GET")
            {
                var dbName = pathSplitted[2];
                if(!Database.AllDatabases.ContainsKey(dbName))
                    return new HttpResponse(HttpStatusCode.NotFound);

                var database = Database.AllDatabases[dbName];
                if (pathSplitted.Length == 3)
                {
                    var result = new { database.Name, Tables = database.Tables.Values.Select(t => new { t.Name, url = "/databases/" + database.Name + "/" + t.Name }) };
                    return new HttpResponse(HttpStatusCode.OK, System.Text.Json.JsonSerializer.Serialize(result));
                }
                else
                {
                    var tableName = pathSplitted[3];
                    if (!database.Tables.ContainsKey(tableName))
                        return new HttpResponse(HttpStatusCode.NotFound);

                    var table = database.Tables[tableName];
                    var result = new { table.Name, url = "/databases/" + database.Name + "/" + table.Name, columns=table.Columns } ;
                    return new HttpResponse(HttpStatusCode.OK, System.Text.Json.JsonSerializer.Serialize(result));

                }

            }
            else
            {
                return new HttpResponse(HttpStatusCode.NotFound);
            }
        }
    }
}