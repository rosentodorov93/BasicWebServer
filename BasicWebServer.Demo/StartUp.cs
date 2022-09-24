namespace BasicWebServer.Demo 
{
    using BasicWebServer.Demo.Controllers;
    using BasicWebServer.Server;
    using BasicWebServer.Server.Http;
    using BasicWebServer.Server.Controllers;
    using System;
    using System.Text;
    using System.Web;

    public class StartUp
    {
        
        
        private const string LoginForm = @"<form action='/Login' method='POST'>
   Username: <input type='text' name='Username'/>
   Password: <input type='text' name='Password'/>
   <input type='submit' value ='Log In' /> 
</form>";
        

        private const string Username = "user";
        private const string Password = "user123";
        static async Task Main(string[] args)
        {
            

            var server = new HttpServer(routes => routes
              .MapGet<HomeController>("/", c => c.Index())
              .MapGet<HomeController>("/Redirect", c => c.Redirect())
              .MapGet<HomeController>("/Html", c => c.Html())
              .MapPost<HomeController>("/Html", c => c.HtmlFormPost())
              .MapGet<HomeController>("/Content", c => c.Content())
              .MapPost<HomeController>("/Content", c => c.DownloadContent())
              .MapGet<HomeController>("/Cookie", c => c.Cookie()));

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
        
    }
}
