using Newtonsoft.Json;
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
        public TextMassage() { } //лучше метод fromString вставить в конструктор класса

        public void fromString(string json)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented

            };
            TextMassage textMassage = Newtonsoft.Json.JsonConvert.DeserializeObject<TextMassage>(json);//, settings);
            this.ReceiverName = textMassage.ReceiverName;
            this.SenderName = textMassage.SenderName;
            this.Text = textMassage.Text;
            this.Time = textMassage.Time;
            this.Contact = textMassage.Contact; 
        }
    }
}
