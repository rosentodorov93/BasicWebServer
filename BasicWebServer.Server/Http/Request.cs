using System.Web;

namespace BasicWebServer.Server.Http
{
    public class Request
    {
        public Method Method { get; private set; }
        public string Url { get; private set; }
        public HeaderCollection Headers { get; private set; }
        public CookieCollection Cookies { get; private set; }
        private static Dictionary<string, Session> Sessions = new Dictionary<string, Session>();
        public string Body { get; private set; }
        public Session Session { get; private set; }
        public IReadOnlyDictionary<string,string> Form { get; private set; }
        public static Request Parse(string request)
        {
            var lines = request.Split("\r\n");

            var startLine = lines[0].Split();

            var method = ParseMethod(startLine[0]);
            var url = startLine[1];
            var headers = ParseHeaders(lines.Skip(1));
            var cookies = ParseCookies(headers);
            var session = GetSession(cookies);

            var bodyLines = lines.Skip(headers.Count + 2).ToArray();
            var body = string.Join("\r\n", bodyLines);
            var form = ParseForm(headers, body);

            return new Request
            {
                Method = method,
                Url = url,
                Headers = headers,
                Cookies = cookies,
                Session = session,
                Body = body,
                Form = form
            };
        }

     

        private static Method ParseMethod(string method)
        {
            try
            {
                return (Method)Enum.Parse(typeof(Method), method, true);
            }
            catch (Exception)
            {
                throw new InvalidOperationException($"Method {method} is not supported!");
            }
        }
        private static HeaderCollection ParseHeaders(IEnumerable<string> headerLines)
        {
            var headerCollection = new HeaderCollection();

            foreach (var line in headerLines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }
                var headerParts = line.Split(":",2);

                if (headerParts.Length != 2)
                {
                    throw new InvalidOperationException("Request is not valid");
                }

                var name = headerParts[0];
                var value = headerParts[1].Trim();

                headerCollection.Add(name,value);
            }

            return headerCollection;
        }
        private static CookieCollection ParseCookies(HeaderCollection headers)
        {
            var cookieCollection = new CookieCollection();

            if (headers.Contains(Header.Cookie))
            {
                var cookieHeader = headers[Header.Cookie];

                var allCookies = cookieHeader.Split(";");

                foreach (var cookie in allCookies)
                {
                    var cookieParts = cookie.Split("=");

                    var name = cookieParts[0].Trim();
                    var value = cookieParts[1].Trim();

                    cookieCollection.Add(name,value);
                }
            }
            return cookieCollection;
        }
        private static Session GetSession(CookieCollection cookies)
        {
            var sessionId = cookies.Contains(Session.SessionCookieName)
                ? cookies[Session.SessionCookieName]
                :Guid.NewGuid().ToString();

            if (!Sessions.ContainsKey(sessionId))
            {
                Sessions[sessionId] = new Session(sessionId);
            }

            return Sessions[sessionId];
        }
        private static Dictionary<string,string> ParseForm(HeaderCollection headers, string body)
        {
            var formCollection = new Dictionary<string,string>();

            if (headers.Contains(Header.ContentType)
                && headers[Header.ContentType] == ContentType.FormUrlEncoded)
            {
                var parsedFormData = ParseFormData(body);

                foreach (var (name, value) in parsedFormData)
                {
                    formCollection.Add(name, value);
                }
            }

            return formCollection;
        }

        private static Dictionary<string, string> ParseFormData(string body)
            => HttpUtility.UrlDecode(body)
            .Split("&")
            .Select(x => x.Split("="))
            .Where(x => x.Length == 2)
            .ToDictionary(
                part => part[0],
                part => part[1],
                StringComparer.InvariantCultureIgnoreCase);
    }
}
