using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PaenkoDB
{
    public class Node
    {
        public Location NodeLocation { get; set; }
        public enum Method { Post, Put }
        public enum Command { Begin, Commit, Rollback }
        public string CurrentTransaction { get; set; } = "NONE";

        public Node(IPAddress ip, int port, bool lookup = false)
        {
            if (lookup)
            {
                NodeLocation = Location.Lookup(ip.ToString(), port);
            }
            else
            {
                NodeLocation = new Location() { ip = ip.ToString(), HttpPort = port };
            }
        }

        public List<string> GetLogs()
        {
            string response = NetworkHandler.Get(this.NodeLocation.HttpAddress(), $"meta/logs");
            List<string> logs = response.Split('\n').ToList();
            logs.Remove(logs.First());
            return logs;
        }

        async public Task<List<string>> GetLogsAsync()
        {
            return await Task.Factory.StartNew<List<string>>(() => GetLogs());
        }


        public List<string> GetKeys(UuidObject log)
        {
            string response = NetworkHandler.Get(this.NodeLocation.HttpAddress(), $"meta/log/{log.Id}/documents");
            Console.WriteLine(response);
            List<string> keys = JsonConvert.DeserializeObject<List<string>>(response);
            return keys;
        }

        async public Task<List<string>> GetKeysAsync(UuidObject log)
        {
            return await Task.Factory.StartNew<List<string>>(() => GetKeys(log));
        }

        public Document GetDocument(UuidObject log, UuidObject file)
        {
            string response = NetworkHandler.Get(this.NodeLocation.HttpAddress(), $"document/{log.Id}/{file.Id}");
            var doc = JsonConvert.DeserializeObject<Document>(response);
            return doc;
        }
        
        async public Task<Document> GetDocumentAsync(UuidObject log, UuidObject file)
        {
            return await Task.Factory.StartNew<Document>(() => GetDocument(log, file));
        }

        public string DeleteDocument(UuidObject log, UuidObject file)
        {
            return NetworkHandler.Delete(this.NodeLocation.HttpAddress(), $"document/{log.Id}/{file.Id}");
        }
        async public Task<string> DeleteDocumentAsync(UuidObject log, UuidObject file)
        {
            return await Task.Factory.StartNew<string>(() => DeleteDocument(log, file));
        }
        public void PostDocument(Document doc, UuidObject log, string addedUuidDescription, Method method = Method.Post)
        {
            string transaction = (CurrentTransaction == "NONE") ? "" : $"/transaction/{CurrentTransaction}";
            string json = JsonConvert.SerializeObject(doc);
            string resp;
            if (method == Method.Post) { resp = NetworkHandler.Send(this.NodeLocation.HttpAddress(), $"document/{log.Id}{transaction}", json, "POST"); }
            else { resp = NetworkHandler.Send(this.NodeLocation.HttpAddress(), $"document/{log.Id}{transaction}", json, "PUT"); }
            UuidManager.Add(resp, addedUuidDescription, UuidObject.UuidType.Key);
        }

        async public Task PostDocumentAsync(Document doc, UuidObject log, string addedUuidDescription, Method method = Method.Post)
        {
            await Task.Factory.StartNew(() => PostDocument(doc, log, addedUuidDescription, method));
        }
        public void Login(Authentication auth)
        {
            string json = JsonConvert.SerializeObject(auth);
            NetworkHandler.Send(this.NodeLocation.HttpAddress(), $"auth/login", json, "POST");
        }
        public void Logout()
        {
            NetworkHandler.Send(this.NodeLocation.HttpAddress(), $"auth/logout", "", "POST");
        }
        public void Transaction(UuidObject log, Command command)
        {
            string resp = NetworkHandler.Send(this.NodeLocation.HttpAddress(), $"transaction/{command.ToString("g").ToLower()}/{log.Id}", "", "POST");
            if (command == Command.Begin) { CurrentTransaction = resp; }
            else { CurrentTransaction = "NONE"; }
        }
    }
}
