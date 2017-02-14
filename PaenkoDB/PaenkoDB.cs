using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
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
        public enum Command { Begin, Commit, Rollback }
        public string LogID { get; set; }

        async public Task<List<PaenkoNode>> CheckNodeStatusAsync(List<PaenkoNode> checkList)
        {
            List<PaenkoNode> Dead = new List<PaenkoNode>();
            foreach (PaenkoNode pn in checkList)
            {
                if (!(await NetworkHandler.CheckAliveAsync(pn.NodeLocation.HttpAddress())))
                {
                    Dead.Add(pn);
                }
            }
            return Dead;
        }

        public List<string> GetLogs(PaenkoNode publicNode)
        {
            string response = NetworkHandler.Get(publicNode.NodeLocation.HttpAddress(), $"meta/logs");
            List<string> logs = response.Split('\n').ToList();
            logs.Remove(logs.First());
            return logs;
        }

        async public Task<List<string>> GetLogsAsync(PaenkoNode publicNode)
        {
            string response = await NetworkHandler.GetAsync(publicNode.NodeLocation.HttpAddress(), $"meta/logs");
            return await Task.Factory.StartNew(() =>
            {
                var logs = response.Split('\n').ToList();
                logs.Remove(logs.First());
                return logs;
            });
        }

        public List<string> GetKeys(PaenkoNode publicNode)
        {
            string response = NetworkHandler.Get(publicNode.NodeLocation.HttpAddress(), $"meta/log/{LogID}/documents");
            Console.WriteLine(response);
            List<string> keys = JsonConvert.DeserializeObject<List<string>>(response);
            return keys;
        }

        async public Task<List<string>> GetKeysAsync(PaenkoNode publicNode)
        {
            string response = await NetworkHandler.GetAsync(publicNode.NodeLocation.HttpAddress(), $"meta/log/{LogID}/documents");
            return await Task.Factory.StartNew(() => {
                var keys = JsonConvert.DeserializeObject<List<string>>(response);
                return keys;
            });
        }
        public PaenkoResponse GetDocument(PaenkoNode publicNode, string fileID)
        {
            string response = NetworkHandler.Get(publicNode.NodeLocation.HttpAddress(), $"document/{LogID}/{fileID}");
            var doc = JsonConvert.DeserializeObject<PaenkoDocument>(response);
            PaenkoResponse _return = new PaenkoResponse() { ErrorMessage = Error.OK, Document = doc, RAW = response };
            return _return;
        }

        async public Task<PaenkoResponse> GetDocumentAsync(PaenkoNode publicNode, string fileID)
        {
            string response = await NetworkHandler.GetAsync(publicNode.NodeLocation.HttpAddress(), $"document/{LogID}/{fileID}");
            Task<PaenkoDocument> deSe = Task<PaenkoDocument>.Factory.StartNew(() =>
            {
                return JsonConvert.DeserializeObject<PaenkoDocument>(response);
            });
            PaenkoResponse _return = new PaenkoResponse() { ErrorMessage = Error.OK, Document = await deSe, RAW = response };
            return _return;
        }

        public PaenkoResponse DeleteDocument(PaenkoNode publicNode, string fileID)
        {
            string response = NetworkHandler.Delete(publicNode.NodeLocation.HttpAddress(), $"document/{LogID}/{fileID}");
            PaenkoDocument PaenkoDoc = new PaenkoDocument();
            PaenkoResponse _return = new PaenkoResponse() { ErrorMessage = Error.OK, Document = PaenkoDoc, RAW = response };
            return _return;
        }

        async public Task<PaenkoResponse> DeleteDocumentAsync(PaenkoNode publicNode, string fileID)
        {
            string response = await NetworkHandler.DeleteAsync(publicNode.NodeLocation.HttpAddress(), $"document/{LogID}/{fileID}");
            PaenkoDocument PaenkoDoc = new PaenkoDocument();
            PaenkoResponse _return = new PaenkoResponse() { ErrorMessage = Error.OK, Document = PaenkoDoc, RAW = response };
            return _return;
        }

        public PaenkoResponse PostDocument(PaenkoNode publicNode, PaenkoDocument doc, Method method, string tid="0")
        {
            string transaction = (tid == "0") ? "" : $"/transaction/{tid}";
            string json = JsonConvert.SerializeObject(doc);
            string resp;
            if (method == Method.Post) { resp = NetworkHandler.Send(publicNode.NodeLocation.HttpAddress(), $"document/{LogID}{transaction}", json, "POST"); }
            else { resp = NetworkHandler.Send(publicNode.NodeLocation.HttpAddress(), $"document/{LogID}{transaction}", json, "PUT"); }
            PaenkoResponse _return = new PaenkoResponse() { ErrorMessage = Error.OK, Document = doc, RAW = resp }; // Set Document to GET for meta info
            return _return;
        }

        async public Task<PaenkoResponse> PostDocumentAsync(PaenkoNode publicNode, PaenkoDocument doc, Method method, string tid = "0")
        {
            string transaction = (tid == "0") ? "" : $"/transaction/{tid}";
            Task<string> jsonTask = Task<string>.Factory.StartNew(
                () => {
                    return JsonConvert.SerializeObject(doc);
                }
                );
            string resp;
            string json = await jsonTask;
            if (method == Method.Post) { resp = await NetworkHandler.SendAsync(publicNode.NodeLocation.HttpAddress(), $"document/{LogID}{transaction}", json, "POST"); }
            else { resp = await NetworkHandler.SendAsync(publicNode.NodeLocation.HttpAddress(), $"document/{LogID}{transaction}", json, "PUT"); }
            PaenkoResponse _return = new PaenkoResponse() { ErrorMessage = Error.OK, Document = doc, RAW = resp }; // Set Document to GET for meta info
            return _return;
        }

        public PaenkoResponse Transaction(PaenkoNode publicNode, Command command)
        {
            string response = NetworkHandler.Send(publicNode.NodeLocation.HttpAddress(), $"transaction/{command.ToString("g").ToLower()}/{LogID}", "", "POST");
            return new PaenkoResponse() { ErrorMessage = Error.OK, Document = null, RAW = response };
        }

        public T GetDocumentAs<T>(PaenkoNode publicNode, string fileID)
        {
            T _return;
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(GetDocument(publicNode, fileID).Document.payload)))
            {
                IFormatter f = new BinaryFormatter();
                _return = (T)f.Deserialize(ms);
            }
            return _return;
        }

        public PaenkoResponse PostDocumentAs<T>(PaenkoNode publicNode, T docobj, Method method, string tid = "0")
        {
            PaenkoDocument doc = new PaenkoDocument() { version = 1 };
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter f = new BinaryFormatter();
                f.Serialize(ms, docobj);
                doc.payload = Convert.ToBase64String(ms.ToArray());
            }
            return PostDocument(publicNode, doc, method, tid);
        }

        public async Task<T> GetDocumentAsAsync<T>(PaenkoNode publicNode, string fileID)
        {
            return await Task.Factory.StartNew(() =>
            {
                T _return;
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(GetDocument(publicNode, fileID).Document.payload)))
                {
                    IFormatter f = new BinaryFormatter();
                    _return = (T)f.Deserialize(ms);
                }
                return _return;
            });
        }

        public async Task<PaenkoResponse> PostDocumentAsAsync<T>(PaenkoNode publicNode, T docobj, Method method, string tid = "0")
        {
            PaenkoDocument doc = new PaenkoDocument() { version = 1 };
            return await Task.Factory.StartNew(() =>
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    IFormatter f = new BinaryFormatter();
                    f.Serialize(ms, docobj);
                    doc.payload = Convert.ToBase64String(ms.ToArray());
                }
                return PostDocument(publicNode, doc, method, tid);
            });
        }
    }
}
