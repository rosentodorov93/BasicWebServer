namespace BasicWebServer.Server
{
    using BasicWebServer.Server.Common;
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
        public readonly IServiceCollection ServiceCollection;


        public HttpServer(string ipAddress, int port, Action<IRoutingTable> routingTableConfiguraion)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;

            this.listener = new TcpListener(this.ipAddress,port);

            routingTableConfiguraion(this.routingTable = new RoutingTable());
            ServiceCollection = new ServiceCollection();
        }
        public HttpServer(int port, Action<IRoutingTable> routingTable)
            :this("127.0.0.1",port,routingTable)
        {

        }
        public HttpServer(Action<IRoutingTable> routingTable)
            :this(9090,routingTable)
        {

        }
        public async Task Start()
        {
            this.listener.Start();

            Console.WriteLine($"Server started at {port}");
            Console.WriteLine("Waiting for requests...");

            while (true)
            {
                var connection = await listener.AcceptTcpClientAsync();

                _ = Task.Run(async () =>
                {
                    var networkStream = connection.GetStream();

                    var requestText = await this.ReadRequest(networkStream);

                    Console.WriteLine(requestText);

                    var request = Request.Parse(requestText, ServiceCollection);

                    var response = this.routingTable.MatchRequest(request);

                    AddSession(request, response);

                    await Writeresponse(networkStream, response);

                    connection.Close();
                });
            }
        }
        private async Task Writeresponse(NetworkStream stream, Response response)
        {
            var responseBytes = Encoding.UTF8.GetBytes(response.ToString());

            await stream.WriteAsync(responseBytes);
        }

        private async Task<string> ReadRequest(NetworkStream networkStream)
        {
            var bufferLength = 1024;
            var buffer = new byte[bufferLength];
            var totalBytesRead = 0;

            var requestBuilder = new StringBuilder();

            do
            {
                var bytesRead = await networkStream.ReadAsync(buffer, 0, bufferLength);
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
        private static void AddSession(Request request, Response response)
        {
            var sessionExists = request.Session.ContainsKey(Session.SessionCurrentDateKey);

            if (!sessionExists)
            {
                request.Session[Session.SessionCurrentDateKey] = DateTime.Now.ToString();
                response.Cookies.Add(Session.SessionCookieName, request.Session.Id);
            }
        }
    }
}
