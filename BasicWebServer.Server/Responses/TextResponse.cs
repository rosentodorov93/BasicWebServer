using BasicWebServer.Server.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Responses
{
    public class TextResponse : ContentResponse
    {
        public TextResponse(string content, Action<Request, Response> preRenderAction = null)
            : base(content, ContentType.PlainText, preRenderAction)
        {
        }
    }
}
