namespace BasicWebServer.Demo 
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
        private const string LoginForm = @"<form action='/Login' method='POST'>
   Username: <input type='text' name='Username'/>
   Password: <input type='text' name='Password'/>
   <input type='submit' value ='Log In' /> 
</form>";
        private const string FileName = "content.txt";

        private const string Username = "user";
        private const string Password = "user123";
        static async Task Main(string[] args)
        {
            await DownloadSitesAsTextFile(FileName, new string[] { "https://judge.softuni.org/", "https://softuni.bg/" });

            var server = new HttpServer(routes => routes
              .MapGet("/", new TextResponse("Hello from the server!"))
              .MapGet("/Cats", new HtmlResponse("Hello from the cats!"))
              .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
              .MapGet("/Html", new HtmlResponse(HtmlForm))
              .MapPost("/Html", new TextResponse("", AddFormDataAction))
              .MapGet("/Content", new HtmlResponse(DownloadForm))
              .MapPost("/Content", new TextFileResponse(FileName))
              .MapGet("/Cookies", new HtmlResponse("",AddCookieAction))
              .MapGet("/Session", new TextResponse("", DisplaySessionInfoAction))
              .MapGet("/Login", new HtmlResponse(LoginForm))
              .MapPost("/Login", new HtmlResponse("", LoginAction))
              .MapGet("/Logout", new HtmlResponse("", LogoutAction))
              .MapGet("/UserProfile", new HtmlResponse("", GetUserDataAction)));

            await server.Start();
        }

        private static void GetUserDataAction(Request request, Response response)
        {
            if (request.Session.ContainsKey(Session.SessionUserKey))
            {
                response.Body = "";
                response.Body += $"<h3>Currently logged in user" + $" is with username - {Username}</h3>";
            }
            else
            {
                response.Body = "";
                response.Body += "<h3>You should first log in " + " - <a href='/Login'>Login</a></h3>";
            }
        }

        private static void LoginAction(Request request, Response response)
        {
            request.Session.Clear();

            var bodyText = "";

            var usernameMatches = request.Form["Username"] == Username;
            var passwordMatches = request.Form["Password"] == Password;

            if (usernameMatches && passwordMatches)
            {
                request.Session[Session.SessionUserKey] = "MyUserId";
                response.Cookies.Add(Session.SessionCookieName, request.Session.Id);

                bodyText = "<h3>Logged in successfully!</h3>";
            }
            else
            {
                bodyText = LoginForm;
            }

            response.Body = "";
            response.Body = bodyText;
        }
        private static void LogoutAction(Request request, Response response)
        {
            request.Session.Clear();

            response.Body = "";
            response.Body = "<h3>Logged out successfully!</h3>";
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
            var requestHasCookies = request.Cookies.Any(c => c.Name != Session.SessionCookieName);
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
        private static void DisplaySessionInfoAction(Request request, Response response)
        {
            var sessionExists = request.Session.ContainsKey(Session.SessionCurrentDateKey);

            var bodyText = "";

            if (sessionExists)
            {
                var currentDate = request.Session[Session.SessionCurrentDateKey];
                bodyText = $"Stored date: {currentDate}!";
            }
            else
            {
                bodyText = "Current date stored!";
            }

            response.Body = "";
            response.Body = bodyText;
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
