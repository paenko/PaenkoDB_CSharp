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
        public string RAW { get; set; }
    }

    public class PaenkoDB
    {
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
            return await Task.Factory.StartNew<List<string>>(() => GetLogs(publicNode));
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
            return await Task.Factory.StartNew<List<string>>(() => GetKeys(publicNode));
        }
        public PaenkoResponse GetDocument(PaenkoNode publicNode, string fileID)
        {
            string response = NetworkHandler.Get(publicNode.NodeLocation.HttpAddress(), $"document/{LogID}/{fileID}");
            var doc = JsonConvert.DeserializeObject<PaenkoDocument>(response);
            PaenkoResponse _return = new PaenkoResponse() { Document = doc, RAW = response };
            return _return;
        }

        async public Task<PaenkoResponse> GetDocumentAsync(PaenkoNode publicNode, string fileID)
        {
            return await Task.Factory.StartNew<PaenkoResponse>(() => GetDocument(publicNode, fileID));
        }

        public PaenkoResponse DeleteDocument(PaenkoNode publicNode, string fileID)
        {
            string response = NetworkHandler.Delete(publicNode.NodeLocation.HttpAddress(), $"document/{LogID}/{fileID}");
            PaenkoDocument PaenkoDoc = new PaenkoDocument();
            PaenkoResponse _return = new PaenkoResponse() { Document = PaenkoDoc, RAW = response };
            return _return;
        }

        async public Task<PaenkoResponse> DeleteDocumentAsync(PaenkoNode publicNode, string fileID)
        {
            return await Task.Factory.StartNew<PaenkoResponse>(() => DeleteDocument(publicNode, fileID));
        }

        public PaenkoResponse PostDocument(PaenkoNode publicNode, PaenkoDocument doc, Method method, string tid="0")
        {
            string transaction = (tid == "0") ? "" : $"/transaction/{tid}";
            string json = JsonConvert.SerializeObject(doc);
            string resp;
            if (method == Method.Post) { resp = NetworkHandler.Send(publicNode.NodeLocation.HttpAddress(), $"document/{LogID}{transaction}", json, "POST"); }
            else { resp = NetworkHandler.Send(publicNode.NodeLocation.HttpAddress(), $"document/{LogID}{transaction}", json, "PUT"); }
            PaenkoResponse _return = new PaenkoResponse() { Document = doc, RAW = resp }; // Set Document to GET for meta info
            return _return;
        }

        async public Task<PaenkoResponse> PostDocumentAsync(PaenkoNode publicNode, PaenkoDocument doc, Method method, string tid = "0")
        {
            return await Task.Factory.StartNew<PaenkoResponse>(() => PostDocument(publicNode, doc, method, tid));
        }

        public PaenkoResponse Transaction(PaenkoNode publicNode, Command command)
        {
            string response = NetworkHandler.Send(publicNode.NodeLocation.HttpAddress(), $"transaction/{command.ToString("g").ToLower()}/{LogID}", "", "POST");
            return new PaenkoResponse() { Document = null, RAW = response };
        }
    }
}
