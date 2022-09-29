namespace BasicWebServer.Demo
{
    using BasicWebServer.Demo.Controllers;
    using BasicWebServer.Server;
    using BasicWebServer.Server.Routing;

    public class StartUp
    {     
        static async Task Main(string[] args)
        {
            var server = new HttpServer(routes => routes
              .MapGet<HomeController>("/", c => c.Index())
              .MapGet<HomeController>("/Redirect", c => c.Redirect())
              .MapGet<HomeController>("/Html", c => c.Html())
              .MapPost<HomeController>("/Html", c => c.HtmlFormPost())
              .MapGet<HomeController>("/Content", c => c.Content())
              .MapPost<HomeController>("/Content", c => c.DownloadContent())
              .MapGet<HomeController>("/Cookie", c => c.Cookie())
              .MapGet<HomeController>("/Session", c => c.Session())
              .MapGet<UsersController>("/Login", c => c.Login())
              .MapPost<UsersController>("/Login", c => c.LoginUser())
              .MapGet<UsersController>("/Logout", c => c.Logout())
              .MapGet<UsersController>("/UserProfile", c => c.GetUserData()));

            await server.Start();
        }        
    }
}
