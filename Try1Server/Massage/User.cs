using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Try1Server.Massage
{
    [Serializable]
    public class User
    {
        public User(string name, string password) { UserName = name; UserPassword = password; }

        [Newtonsoft.Json.JsonProperty("username")]
        public string UserName { get; set; }
        /*
                [Newtonsoft.Json.JsonProperty("time")]
                public string TimeSpan { get; set; }
        */
        [Newtonsoft.Json.JsonProperty("userpassword")]
        public string UserPassword { get; set; }
    }
}
