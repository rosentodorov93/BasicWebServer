using BasicWebServer.Server.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Routing
{
    public interface IRoutingTable
    {
        IRoutingTable Map(Method method, string url, Func<Request, Response> responseFunction);

        IRoutingTable MapGet(string url, Func<Request, Response> responseFunction);

        IRoutingTable MapPost(string url, Func<Request, Response> responseFunction);
    }
}
