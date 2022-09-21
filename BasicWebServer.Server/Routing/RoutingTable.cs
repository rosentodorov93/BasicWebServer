using BasicWebServer.Server.Common;
using BasicWebServer.Server.Http;
using BasicWebServer.Server.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Routing
{
    public class RoutingTable : IRoutingTable
    {
        private readonly Dictionary<Method, Dictionary<string, Func<Request, Response>>> routes;

        public RoutingTable()
            => this.routes = new()
            {
                [Method.Get] = new(),
                [Method.Post] = new(),
                [Method.Put] = new(),
                [Method.Delete] = new()
            };
        public IRoutingTable Map(Method method, string url, Func<Request, Response> responseFunction)
        {
            Guard.AgainstNull(url, nameof(url));
            Guard.AgainstNull(responseFunction, nameof(responseFunction));

            this.routes[method][url] = responseFunction;

            return this;    

        }

        public IRoutingTable MapGet(string url, Func<Request, Response> responseFunction)
            => Map(Method.Get, url, responseFunction);

        public IRoutingTable MapPost(string url, Func<Request, Response> responseFunction)
            => Map(Method.Post, url, responseFunction);

        public Response MatchRequest(Request request)
        {
            var requestMethod = request.Method;
            var requestUrl = request.Url;

            if (!this.routes.ContainsKey(requestMethod)
                || !this.routes[requestMethod].ContainsKey(requestUrl))
            {
                return new NotFoundResponse();
            }

            var responseFunction =  this.routes[requestMethod][requestUrl];

            return responseFunction(request);
        }
    }
}
