﻿namespace BasicWebServer.Server.Http
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

        public string Body { get; set; }
    }
}
