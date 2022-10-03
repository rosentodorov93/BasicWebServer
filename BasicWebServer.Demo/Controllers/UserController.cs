using BasicWebServer.Demo.Services;
using BasicWebServer.Server.Attributes;
using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Demo.Controllers
{
    public class UserController : Controller
    {
        private readonly UserServices userServices;
        public UserController(Request request, UserServices _userServices)
            : base(request)
        {
            userServices = _userServices;
        }

        public Response Login() => View();
        [HttpPost]
        public Response LoginUser()
        {
            this.Request.Session.Clear();

            var textResult = "";

            var username = this.Request.Form["Username"];
            var password = this.Request.Form["Password"];

            if (userServices.IsLoginCorrect(username,password))
            {
                if (!this.Request.Session.ContainsKey(Session.SessionUserKey))
                {
                    SignIn(Guid.NewGuid().ToString());

                    var cookies = new CookieCollection();
                    cookies.Add(Session.SessionCookieName, this.Request.Session.Id);
                    return Html("<h3>Logged in successfully!</h3>", cookies);
                }


                return Html("<h3>Logged in successfully!</h3>");
            }

            return Redirect("/Login");
        }
        public Response Logout()
        {
            SignOut();

            return Html("<h3>Logged out successfully!</h3>");
        }
        [Authorize]
        public Response GetUserData()
        {
            return Html($"<h3>Currently logged in user" + $" is with Id - {User.Id}</h3>");
        }
    }
}
