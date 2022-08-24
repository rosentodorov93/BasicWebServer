namespace BasicWebServer.Server
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
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

                var requestText = this.ReadRequest(networkStream);

                Console.WriteLine(requestText);

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

        private string ReadRequest(NetworkStream networkStream)
        {
            var bufferLength = 1024;
            var buffer = new byte[bufferLength];
            var totalBytesRead = 0;

            var requestBuilder = new StringBuilder();

            do
            {
                var bytesRead = networkStream.Read(buffer, 0, bufferLength);
                totalBytesRead += bytesRead;

                if (totalBytesRead > 10 * 1024)
                {
                    throw new InvalidOperationException("Request is too large!");
                }

                requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            } 
            while (networkStream.DataAvailable);

            return requestBuilder.ToString();
        }
    }
}
