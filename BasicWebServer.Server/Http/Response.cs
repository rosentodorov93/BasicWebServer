using System.Text;

namespace BasicWebServer.Server.Http
{
    public class Response
    {
        public Response(StatusCode statusCode)
        {
            StatusCode = statusCode;
            this.Headers.Add(Header.Server,"My Web Server");
            this.Headers.Add(Header.Date,$"{DateTime.UtcNow:r}");
        }

        public StatusCode StatusCode { get; init; }

        public HeaderCollection Headers { get; } = new HeaderCollection();
        public CookieCollection Cookies { get; } = new CookieCollection();

        public string Body { get; set; }

        public Action<Request,Response> PreRenderAction { get; protected set; }
        public override string ToString()
        {
            var responseBuilder = new StringBuilder();

            responseBuilder.AppendLine($"HTTP/1.1 {(int)this.StatusCode} {this.StatusCode}");

            foreach (var header in this.Headers)
            {
                responseBuilder.AppendLine(header.ToString());
            }

            foreach (var cookie in this.Cookies)
            {
                responseBuilder.AppendLine($"{Header.SetCookie}: {cookie.ToString()}");
            }
            responseBuilder.AppendLine();

            if (!string.IsNullOrEmpty(this.Body))
            {
                responseBuilder.AppendLine(this.Body);
            }

            return responseBuilder.ToString();
        }
    }
}
