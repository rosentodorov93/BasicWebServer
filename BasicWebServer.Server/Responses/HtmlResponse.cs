using BasicWebServer.Server.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Responses
{
    public class HtmlResponse : ContentResponse
    {
        public HtmlResponse(string content, Action<Request, Response> preRenderAction = null) 
            : base(content, ContentType.Html, preRenderAction)
        {
        }
    }
}
