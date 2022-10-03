using BasicWebServer.Demo.Models;
using BasicWebServer.Server.Attributes;
using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BasicWebServer.Demo.Controllers
{
    public class HomeController : Controller
    {
        private const string FileName = "content.txt";

        public HomeController(Request request) 
            : base(request)
        {
        }

        public Response Index() => Text("Hello from the server!");
        public Response Student(string name, int age) => Text($"Hello I am {name} and i am {age} years old!");
        public Response Test()
        {
            var model = new List<FormViewModel>
            {
                new(){Name = "Rosko", Age = 29},
                new(){Name = "Pencho", Age = 29},
                new(){Name = "Plami", Age = 27}
            };

            return View(model);
        }
        public Response Redirect() => Redirect("https://softuni.bg");
        public Response Html() => View();

        [HttpPost]
        public Response HtmlFormPost()
        {
            var name = this.Request.Form["Name"];
            var age = this.Request.Form["Age"];

            var model = new FormViewModel()
            {
                Name = name,
                Age = int.Parse(age)
            };

            return View(model);
        }
        public Response Content() => View();

        [HttpPost]
        public Response DownloadContent()
        {
            DownloadSitesAsTextFile(FileName, new string[] { "https://judge.softuni.org/", "https://softuni.bg/" }).Wait();

            return File(FileName);
        }
        public Response Cookie()
        {
            var resultHtml = "";
            var cookies = new CookieCollection();
            if (this.Request.Cookies.Any(c => c.Name != BasicWebServer.Server.Http.Session.SessionCookieName))
            {
                var cookieBuilder = new StringBuilder();
                cookieBuilder.AppendLine("<h1>Cookies</h1>");

                cookieBuilder.Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");

                foreach (var cookie in this.Request.Cookies)
                {
                    cookieBuilder.Append("<tr>");
                    cookieBuilder.Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
                    cookieBuilder.Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");
                    cookieBuilder.Append("</tr>");
                }
                cookieBuilder.Append("</table>");

                resultHtml = cookieBuilder.ToString();
            }
            else
            {
                cookies.Add("My-Cookie", "My-Value");
                cookies.Add("My-Second-Cookie", "My-Second-Value");
                resultHtml = "<h1>Cookies Set!</h1>";
            }

            return Html(resultHtml,cookies);

        }
        public Response Session()
        {
            var currentDateKey = "CurrentDate";
            var sessionExists = this.Request.Session.ContainsKey(currentDateKey);
            var textResult = "";
            if (sessionExists)
            {
                var currentDate = this.Request.Session[currentDateKey];
                textResult = $"Stored date: {currentDate}!";
            }
            else
            {
                textResult = "Current date stored!";
            }

            return Text(textResult);
        }
        private static async Task<string> DownloadWebSiteContent(string url)
        {
            var client = new HttpClient();

            using (client)
            {
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                return content.Substring(0, 2000);
            }
        }
        private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
        {
            var downloads = new List<Task<string>>();

            foreach (var url in urls)
            {
                downloads.Add(DownloadWebSiteContent(url));
            }

            var responses = await Task.WhenAll(downloads);

            var joinedResponses = string.Join(Environment.NewLine + new String('-', 50), responses);

            await System.IO.File.WriteAllTextAsync(fileName, joinedResponses);
        }
    }
}
