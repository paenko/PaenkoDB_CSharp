using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaenkoDB
{
    public class Authentication
    {
        public string username { get; set; }
        public string password { get; set; }

        /// <summary>
        /// Create an Authentication object to use when starting a session
        /// </summary>
        /// <param name="username">Database config username</param>
        /// <param name="password">Database config password</param>
        public Authentication(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
