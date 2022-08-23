using BasicWebServer.Server;
using System;

namespace BasicWebServer.Demo 
{
    internal class StartUp
    {
        static void Main(string[] args)
        {
            var server = new HttpServer("127.0.0.1", 9090);
            server.Start();
        }
    }
}
