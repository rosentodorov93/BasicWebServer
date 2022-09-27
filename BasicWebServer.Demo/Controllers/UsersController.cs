﻿using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Demo.Controllers
{
    internal class UsersController : Controller
    {
        private const string LoginForm = @"<form action='/Login' method='POST'>
   Username: <input type='text' name='Username'/>
   Password: <input type='text' name='Password'/>
   <input type='submit' value ='Log In' /> 
</form>";

        private const string Username = "user";
        private const string Password = "user123";
        public UsersController(Request request)
            : base(request)
        {
        }

        public Response Login() => Html(LoginForm);
        
        public Response LoginUser()
        {
            this.Request.Session.Clear();

            var textResult = "";

            var usernameMatches = this.Request.Form["Username"] == Username;
            var passwordMatches = this.Request.Form["Password"] == Password;

            if (usernameMatches && passwordMatches)
            {
                if (!this.Request.Session.ContainsKey(Session.SessionUserKey))
                {
                    this.Request.Session[Session.SessionUserKey] = "MyUserId";

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
            this.Request.Session.Clear();

            return Html("<h3>Logged out successfully!</h3>");
        }
        public Response GetUserData()
        {
            if (this.Request.Session.ContainsKey(Session.SessionUserKey))
            {
                return Html($"<h3>Currently logged in user" + $" is with username - {Username}</h3>");
            }

            return Redirect("/Login");
        }
    }
}
