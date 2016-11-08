using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaenkoDB
{
    public class PaenkoDocument
    {
        public string payload { get; set; }
        public uint version { get; set; }
        public string id { get; set; }
        //public string username { get; set; }
        //public string password { get; set; }

        static public PaenkoDocument Open(string filePath, string user, string pass)
        {
            byte[] content = FileHandler.ReadFile(filePath);
            return (new PaenkoDocument() { payload = Convert.ToBase64String(content), version = 1 });//, password = pass, username = user } );
        }
        static public PaenkoDocument FromContent(string source, string user, string pass)
        {
            byte[] content = Encoding.Default.GetBytes(source);
            return (new PaenkoDocument() { payload = Convert.ToBase64String(content), version = 1 });//, password = pass, username = user } );
        }
    }
}
