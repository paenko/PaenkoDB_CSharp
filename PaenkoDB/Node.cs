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

        /// <summary>
        /// Node constructor
        /// </summary>
        /// <param name="ip">IP address of the database server</param>
        /// <param name="port">HTTP port of the database server</param>
        /// <param name="lookup">Enable Geotracking</param>
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

        /// <summary>
        /// Get all the logs on this database server
        /// </summary>
        /// <returns>A list of all logs that are currently available on the database server</returns>
        public List<string> GetLogs()
        {
            string response = NetworkHandler.Get(this.NodeLocation.HttpAddress(), $"meta/logs");
            List<string> logs = response.Split('\n').ToList();
            logs.Remove(logs.First());
            return logs;
        }

        /// <summary>
        /// Get all the logs on this database server asynchronously
        /// </summary>
        /// <returns>A list of all logs that are currently available on the database server</returns>
        async public Task<List<string>> GetLogsAsync()
        {
            return await Task.Factory.StartNew<List<string>>(() => GetLogs());
        }

        /// <summary>
        /// Get all the keys on a specified log on this database server
        /// </summary>
        /// <param name="log">The log you want the keys from</param>
        /// <returns>A list of all keys that are currently available on the specified log on the database server</returns>
        public List<string> GetKeys(UuidObject log)
        {
            string response = NetworkHandler.Get(this.NodeLocation.HttpAddress(), $"meta/log/{log.Id}/documents");
            Console.WriteLine(response);
            List<string> keys = JsonConvert.DeserializeObject<List<string>>(response);
            return keys;
        }

        /// <summary>
        /// Get all the keys on a specified log on this database server asynchronously
        /// </summary>
        /// <param name="log">The log you want the keys from</param>
        /// <returns>A list of all keys that are currently available on the specified log on the database server</returns>
        async public Task<List<string>> GetKeysAsync(UuidObject log)
        {
            return await Task.Factory.StartNew<List<string>>(() => GetKeys(log));
        }

        /// <summary>
        /// Get a specified document from the database server
        /// </summary>
        /// <param name="log">The log that stores your document</param>
        /// <param name="file">The key of your document</param>
        /// <returns>A document</returns>
        public Document GetDocument(UuidObject log, UuidObject file)
        {
            string response = NetworkHandler.Get(this.NodeLocation.HttpAddress(), $"document/{log.Id}/{file.Id}");
            var doc = JsonConvert.DeserializeObject<Document>(response);
            return doc;
        }

        /// <summary>
        /// Get a specified document from the database server asynchronously
        /// </summary>
        /// <param name="log">The log that stores your document</param>
        /// <param name="file">The key of your document</param>
        /// <returns>A document</returns>
        async public Task<Document> GetDocumentAsync(UuidObject log, UuidObject file)
        {
            return await Task.Factory.StartNew<Document>(() => GetDocument(log, file));
        }

        /// <summary>
        /// Delete a specified document from the database server
        /// </summary>
        /// <param name="log">The log that stores your document</param>
        /// <param name="file">The key of your document</param>
        /// <returns>The server response</returns>
        public string DeleteDocument(UuidObject log, UuidObject file)
        {
            return NetworkHandler.Delete(this.NodeLocation.HttpAddress(), $"document/{log.Id}/{file.Id}");
        }

        /// <summary>
        /// Delete a specified document from the database server asynchronously
        /// </summary>
        /// <param name="log">The log that stores your document</param>
        /// <param name="file">The key of your document</param>
        /// <returns>The server response</returns>
        async public Task<string> DeleteDocumentAsync(UuidObject log, UuidObject file)
        {
            return await Task.Factory.StartNew<string>(() => DeleteDocument(log, file));
        }

        /// <summary>
        /// Send a document to the database server
        /// </summary>
        /// <param name="doc">The document you want to send</param>
        /// <param name="log">The log you want to send your document to</param>
        /// <param name="addedUuidDescription">The description that the automatically generated UuidObject in the Manager will get</param>
        /// <param name="method">Sending method</param>
        public void PostDocument(Document doc, UuidObject log, string addedUuidDescription, Method method = Method.Post)
        {
            string transaction = (CurrentTransaction == "NONE") ? "" : $"/transaction/{CurrentTransaction}";
            string json = JsonConvert.SerializeObject(doc);
            string resp;
            if (method == Method.Post) { resp = NetworkHandler.Send(this.NodeLocation.HttpAddress(), $"document/{log.Id}{transaction}", json, "POST"); }
            else { resp = NetworkHandler.Send(this.NodeLocation.HttpAddress(), $"document/{log.Id}{transaction}", json, "PUT"); }
            UuidManager.Add(resp, addedUuidDescription, UuidObject.UuidType.Key);
        }

        /// <summary>
        /// Send a document to the database server asynchronously
        /// </summary>
        /// <param name="doc">The document you want to send</param>
        /// <param name="log">The log you want to send your document to</param>
        /// <param name="addedUuidDescription">The description that the automatically generated UuidObject in the Manager will get</param>
        /// <param name="method">Sending method</param>
        async public Task PostDocumentAsync(Document doc, UuidObject log, string addedUuidDescription, Method method = Method.Post)
        {
            await Task.Factory.StartNew(() => PostDocument(doc, log, addedUuidDescription, method));
        }

        /// <summary>
        /// Start a session with the database server
        /// </summary>
        /// <param name="auth">Authentication data wich is specified in the config file of your database</param>
        public void Login(Authentication auth)
        {
            string json = JsonConvert.SerializeObject(auth);
            NetworkHandler.Send(this.NodeLocation.HttpAddress(), $"auth/login", json, "POST");
        }

        /// <summary>
        /// End an established session with the database server
        /// </summary>
        public void Logout()
        {
            NetworkHandler.Send(this.NodeLocation.HttpAddress(), $"auth/logout", "", "POST");
        }

        /// <summary>
        /// Start a database transaction
        /// </summary>
        /// <param name="log">The log you want your transaction to target</param>
        /// <param name="command">The transaction function</param>
        public void Transaction(UuidObject log, Command command)
        {
            string resp = NetworkHandler.Send(this.NodeLocation.HttpAddress(), $"transaction/{command.ToString("g").ToLower()}/{log.Id}", "", "POST");
            if (command == Command.Begin) { CurrentTransaction = resp; }
            else { CurrentTransaction = "NONE"; }
        }
    }
}
