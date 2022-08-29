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
        IRoutingTable Map(Method method, string url, Response response);

        IRoutingTable MapGet(string url, Response response);

        IRoutingTable MapPost(string url, Response response);
    }
}
