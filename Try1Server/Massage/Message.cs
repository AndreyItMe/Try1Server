using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Try1Server.Massage
{
    [Serializable] //для сериализации
    public class Message
    {
        
        public new TimeSpan Time { get; set; } //https://learn.microsoft.com/en-us/dotnet/api/system.timespan?view=net-8.0&redirectedfrom=MSDN

        public new Contact Contact { get; set; }
    
        public Message() { }
        public override string ToString()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(this, settings);
            return json;
        }
    }
}
