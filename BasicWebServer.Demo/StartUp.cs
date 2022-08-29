namespace BasicWebServer.Demo 
{
    using BasicWebServer.Server;
    using BasicWebServer.Server.Responses;
    using System;
    internal class StartUp
    {
        static void Main(string[] args)
           => new HttpServer(routes => routes
               .MapGet("/", new TextResponse("Hello from the server!"))
               .MapGet("/Cats", new HtmlResponse("Hello from the cats!"))
               .MapGet("/Redirect", new RedirectResponse("https://softuni.org/")))
           .Start();
    }
}
