namespace BasicWebServer.Demo
{
    using BasicWebServer.Demo.Controllers;
    using BasicWebServer.Server;
    using BasicWebServer.Server.Routing;

    public class StartUp
    {     
        static async Task Main(string[] args)
        {
            await new HttpServer(routes => routes
                .MapControllers())
                .Start();
        }        
    }
}
