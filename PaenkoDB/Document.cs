using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PaenkoDB
{
    public class Document
    {
        public string payload { get; set; }
        public uint version { get; set; }
        public string id { get; set; }

        public T ToObject<T>()
        {
            T _return;
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(payload)))
            {
                IFormatter f = new BinaryFormatter();
                _return = (T)f.Deserialize(ms);
            }
            return _return;
        }

        public static Document FromObject<T>(T docobj)
        {
            Document doc = new Document() { version = 1 };
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter f = new BinaryFormatter();
                f.Serialize(ms, docobj);
                doc.payload = Convert.ToBase64String(ms.ToArray());
            }
            return doc;
        }

        public static Document FromStream(Stream docstream)
        {
            Document doc = new Document() { version = 1 };
            using (MemoryStream ms = new MemoryStream())
            {
                docstream.CopyTo(ms);
                doc.payload = Convert.ToBase64String(ms.ToArray());
            }
            return doc;
        }
    }
}
