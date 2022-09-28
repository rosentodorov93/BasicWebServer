using BasicWebServer.Server.Http;

namespace BasicWebServer.Server.Responses
{
    public class TextResponse : ContentResponse
    {
        public TextResponse(string content)
            : base(content, ContentType.PlainText)
        {
        }
    }
}
