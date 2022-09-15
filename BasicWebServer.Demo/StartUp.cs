﻿namespace BasicWebServer.Demo 
{
    using BasicWebServer.Server;
    using BasicWebServer.Server.Http;
    using BasicWebServer.Server.Responses;
    using System;
    using System.Text;
    using System.Web;

    public class StartUp
    {
        private const string HtmlForm = @"<form action='/Html' method='POST'>
           Name: <input type='text' name='Name'/>
           Age: <input type='number' name ='Age'/>
        <input type='submit' value ='Save' />
        </form>";
        private const string DownloadForm = @"<form action='/Content' method='POST'>
   <input type='submit' value ='Download Sites Content' /> 
</form>";
        private const string FileName = "content.txt";
        static async Task Main(string[] args)
        {
            await DownloadSitesAsTextFile(FileName, new string[] { "https://judge.softuni.org/", "https://softuni.org/" });

            var server = new HttpServer(routes => routes
              .MapGet("/", new TextResponse("Hello from the server!"))
              .MapGet("/Cats", new HtmlResponse("Hello from the cats!"))
              .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
              .MapGet("/Html", new HtmlResponse(HtmlForm))
              .MapPost("/Html", new TextResponse("", StartUp.AddFormDataAction))
              .MapGet("/Content", new HtmlResponse(DownloadForm))
              .MapPost("/Content", new TextFileResponse(FileName))
              .MapGet("/Cookies", new HtmlResponse("",AddCookieAction)));

            await server.Start();
        }
           

        private static void AddFormDataAction(Request request, Response response)
        {
            response.Body = "";

            foreach (var (key, value) in request.Form)
            {
                response.Body += $"{key} - {value}";
                response.Body += Environment.NewLine;
            }
        }
        private static void AddCookieAction(Request request, Response response)
        {
            var requestHasCookies = request.Cookies.Any();
            var bodyText = "";

            if (requestHasCookies)
            {
                var cookieBuilder = new StringBuilder();
                cookieBuilder.AppendLine("<h1>Cookies</h1>");

                cookieBuilder.Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");

                foreach (var cookie in request.Cookies)
                {
                    cookieBuilder.Append("<tr>");
                    cookieBuilder.Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
                    cookieBuilder.Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");
                    cookieBuilder.Append("</tr>");
                }
                cookieBuilder.Append("</table>");

                bodyText = cookieBuilder.ToString();
            }
            else
            {
                bodyText = "<h1>Cookies Set!</h1>";
            }
            response.Body = bodyText;

            if (!requestHasCookies)
            {
                response.Cookies.Add("My-Cookie", "My-Value");
                response.Cookies.Add("My-Second-Cookie", "My-Second-Value");
            }
        }
        private static async Task<string> DownloadWebSiteContent(string url)
        {
            var client = new HttpClient();

            using (client)
            {
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                return content.Substring(0, 2000);
            }
        }
        private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
        {
            var downloads = new List<Task<string>>();

            foreach (var url in urls)
            {
                downloads.Add(DownloadWebSiteContent(url));
            }

            var responses = await Task.WhenAll(downloads);

            var joinedResponses = string.Join(Environment.NewLine + new String('-', 50), responses);

            await File.WriteAllTextAsync(fileName, joinedResponses);
        }
    }
}
