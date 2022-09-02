namespace BasicWebServer.Server
{
    using BasicWebServer.Server.Http;
    using BasicWebServer.Server.Routing;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener listener;

        private readonly RoutingTable routingTable;


        public HttpServer(string ipAddress, int port, Action<IRoutingTable> routingTableConfiguraion)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;

            this.listener = new TcpListener(this.ipAddress,port);

            routingTableConfiguraion(this.routingTable = new RoutingTable());
        }
        public HttpServer(int port, Action<IRoutingTable> routingTable)
            :this("127.0.0.1",port,routingTable)
        {

        }
        public HttpServer(Action<IRoutingTable> routingTable)
            :this(9090,routingTable)
        {

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

                var request = Request.Parse(requestText);

                var response = this.routingTable.MatchRequest(request);

                if (response.PreRenderAction != null)
                {
                    response.PreRenderAction(request, response);
                }

                Writeresponse(networkStream, response);

                connection.Close();
            }
        }

        private void Writeresponse(NetworkStream stream, Response response)
        {
            var responseBytes = Encoding.UTF8.GetBytes(response.ToString());

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
