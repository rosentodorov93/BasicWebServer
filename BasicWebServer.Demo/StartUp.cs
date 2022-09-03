namespace BasicWebServer.Demo 
{
    using BasicWebServer.Server;
    using BasicWebServer.Server.Http;
    using BasicWebServer.Server.Responses;
    using System;
    public class StartUp
    {
        private const string HtmlForm = @"<form action='/Html' method='POST'>
           Name: <input type='text' name='Name'/>
           Age: <input type='number' name ='Age'/>
        <input type='submit' value ='Save' />
        </form>";
        static async Task Main(string[] args)
           => await new HttpServer(routes => routes
               .MapGet("/", new TextResponse("Hello from the server!"))
               .MapGet("/Cats", new HtmlResponse("Hello from the cats!"))
               .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
               .MapGet("/Html", new HtmlResponse(HtmlForm))
               .MapPost("/Html", new TextResponse("",StartUp.AddFormDataAction)))
           .Start();

        private static void AddFormDataAction(Request request, Response response)
        {
            response.Body = "";

            foreach (var (key, value) in request.Form)
            {
                response.Body += $"{key} - {value}";
                response.Body += Environment.NewLine;
            }
        }

    }
}
