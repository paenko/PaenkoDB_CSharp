using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaenkoDB
{
    public class PaenkoResponse
    {
        public PaenkoDocument Document { get; set; }
        public PaenkoDB.Error ErrorMessage { get; set; }
        public string RAW { get; set; }
    }

    public class PaenkoDB
    {
        Random RandomGenerator = new Random();
        public enum Error { ConnectionError, FileError, OK }
        public enum Method { Post, Put }

        public List<PaenkoNode> CheckNodeStatus(List<PaenkoNode> checkList)
        {
            List<PaenkoNode> Dead = new List<PaenkoNode>();
            foreach (PaenkoNode pn in checkList)
            {
                if (!NetworkHandler.CheckAlive(pn.NodeLocation.))
                {
                    Dead.Add(pn);
                }
            }
            return Dead;
        }

        public List<string> GetKeys(PaenkoNode publicNode)
        {
            string response = NetworkHandler.GET(publicNode.NodeLocation.HttpAddress(), $"document");
            List<string> keys = response.Split(';').ToList();
            for (int i = 0; i < keys.Count; i++) keys[i] = keys[i].Split('/').Last();
            keys.Remove(keys.Last());
            return keys;
        }
        public PaenkoResponse GetDocument(PaenkoNode publicNode, string fileID)
        {
            string response = NetworkHandler.GET(publicNode.NodeLocation.HttpAddress(), $"document/{fileID}");
            var doc = JsonConvert.DeserializeObject<PaenkoDocument>(response);
            PaenkoResponse _return = new PaenkoResponse() { ErrorMessage = Error.OK, Document = doc, RAW = response };
            return _return;
        }

        public PaenkoResponse DeleteDocument(PaenkoNode publicNode, string fileID)
        {
            string response = NetworkHandler.DELETE(publicNode.NodeLocation.HttpAddress(), $"document/{fileID}");
            PaenkoDocument PaenkoDoc = new PaenkoDocument();
            PaenkoResponse _return = new PaenkoResponse() { ErrorMessage = Error.OK, Document = PaenkoDoc, RAW = response };
            return _return;
        }

        public PaenkoResponse PostDocument(PaenkoNode publicNode, PaenkoDocument doc, Method method)
        {
            string json = JsonConvert.SerializeObject(doc);
            string resp;
            if (method == Method.Post) { resp = NetworkHandler.Send(publicNode.NodeLocation.HttpAddress(), $"document", json, "POST"); }
            else { resp = NetworkHandler.Send(publicNode.NodeLocation.HttpAddress(), $"document", json, "PUT"); }
            PaenkoResponse _return = new PaenkoResponse() { ErrorMessage = Error.OK, Document = doc, RAW = resp }; // Set Document to GET for meta info
            return _return;
        }
    }
}
