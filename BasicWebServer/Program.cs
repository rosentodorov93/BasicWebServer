using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BasicWebServer 
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");

            int port = 9090;

            var listener = new TcpListener(ipAddress ,port);

            listener.Start();

            Console.WriteLine($"Server started at {port}");
            Console.WriteLine("Waiting for requests...");

            while (true)
            {
                var connection = listener.AcceptTcpClient();

                var networkStream = connection.GetStream();

                var content = "Hello from the server!";
                var contentLength = Encoding.UTF8.GetByteCount(content);

                var response = $@"HTTP/1.1 200 OK
Content-Type: text/plain; charset=UTF-8
Content-Length: {contentLength}

{content}";

                var responseBytes = Encoding.UTF8.GetBytes(response);

                networkStream.Write(responseBytes);

                connection.Close();
            }
        }
    }
}