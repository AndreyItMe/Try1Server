using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Try1Server.Massage
{
    [Serializable] //для сериализации
    public class TextMassage : Message
    {
        [Newtonsoft.Json.JsonProperty("text")]
        public string Text { get; set; }
/*
        [Newtonsoft.Json.JsonProperty("time")]
        public string TimeSpan { get; set; }
*/
        [Newtonsoft.Json.JsonProperty("senderName")] // от кого письмо
        public string SenderName { get; set; }

        [Newtonsoft.Json.JsonProperty("receiverName")] //кому письмо
        public string ReceiverName { get; set; }

        /*
                [Newtonsoft.Json.JsonProperty("time")]
                public new TimeSpan Time { get; set; } //https://learn.microsoft.com/en-us/dotnet/api/system.timespan?view=net-8.0&redirectedfrom=MSDN
        */
        /*
                [Newtonsoft.Json.JsonProperty("contact")]
                public Contact Contact { get; set; }
        */
        /*
                public int text { get; set; }
                public string contact { get; set; }
        */
        /*
                public TextMassage(TimeSpan LastTime, Contact LastContact) : base(LastTime, LastContact) //point - центр круга
                {

                }
        */
        public TextMassage() { }
    }
}
