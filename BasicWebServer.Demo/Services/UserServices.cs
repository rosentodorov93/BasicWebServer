using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Demo.Services
{
    public class UserServices
    {
        private const string Username = "User";
        private const string Password = "User123";

        public bool IsLoginCorrect(string username, string password)
                => username == Username && password == Password;
    }
}
