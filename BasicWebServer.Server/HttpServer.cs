using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server
{
    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener listener;

        public HttpServer(string ipAddress, int port)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;
            this.listener = new TcpListener(this.ipAddress,port);
        }

        public void Start()
        {
            this.listener.Start();

            Console.WriteLine($"Server started at {port}");
            Console.WriteLine("Waiting for requests...");

            while (true)
            {
                var connection = listener.AcceptTcpClient();

                var networkStream = connection.GetStream();

                Writeresponse(networkStream, "Hello from the server!");

                connection.Close();
            }
        }

        private void Writeresponse(NetworkStream stream, string content)
        {
            var contentLength = Encoding.UTF8.GetByteCount(content);

            var response = $@"HTTP/1.1 200 OK
Content-Type: text/plain; charset=UTF-8
Content-Length: {contentLength}

{content}";

            var responseBytes = Encoding.UTF8.GetBytes(response);

            stream.Write(responseBytes);
        }
    }
}
