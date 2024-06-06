using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace Try1Server
{
    [Serializable]
    public class Contact
    {
        public string Name { get; set; }
        public int id { get; set; }

        //public new Bitmap Address { get; set; }

        public new void AcceptRejectRule()
        {
            //Bitmap bmp = null;
        }
/*
        public Contact(string name)
        {
            Name = name;
        }
*/
        public Contact() {}
    }
}
