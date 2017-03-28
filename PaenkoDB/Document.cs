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

        /// <summary>
        /// Deserialize a document to an object
        /// </summary>
        /// <typeparam name="T">The type of the object you want to deserialize</typeparam>
        /// <returns>The deserialized object</returns>
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

        /// <summary>
        /// Create a document from the serialization of an object
        /// </summary>
        /// <typeparam name="T">The type of the object you want to serialize</typeparam>
        /// <returns>A Document with the serialization as payload</returns>
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

        /// <summary>
        /// Create a document from a stream
        /// </summary>
        /// <param name="docstream">The stream which will be used to create the document</param>
        /// <returns>A Document</returns>
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
