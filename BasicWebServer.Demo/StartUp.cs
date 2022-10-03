namespace BasicWebServer.Demo
{
    using BasicWebServer.Demo.Services;
    using BasicWebServer.Server;
    using BasicWebServer.Server.Routing;

    public class StartUp
    {     
        static async Task Main(string[] args)
        {
            var server = new HttpServer(routes => routes.MapControllers());

            server.ServiceCollection.Add<UserServices>();

            await server.Start();
        }        
    }
}
