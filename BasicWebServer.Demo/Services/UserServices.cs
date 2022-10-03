using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Demo.Services
{
    public class UserServices
    {
        private const string Username = "user";
        private const string Password = "user123";

        public bool IsLoginCorrect(string username, string password)
                => username == Username && password == Password;
    }
}
