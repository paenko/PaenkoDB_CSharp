using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PaenkoDB
{
    public class NetworkHandler
    {
        static CookieContainer CookieJar = new CookieContainer();
        public static string Send(string ServerUrl, string dataPath, string json, string method)
        {
            Console.WriteLine(string.Format("{0}{1}", ServerUrl, dataPath));
            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", ServerUrl, dataPath));
            request.CookieContainer = CookieJar;
            request.Method = method;
            request.ContentType = "application/json";
            request.ContentLength = json.Length;

            using (var stream = request.GetRequestStream())
            using (var writeStream = new StreamWriter(stream))
            {
                writeStream.Write(json);
            }
            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;
        }

        public static string Get(string ServerUrl, string dataPath)
        {
            Console.WriteLine(string.Format("{0}{1}", ServerUrl, dataPath));
            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", ServerUrl, dataPath));
            request.CookieContainer = CookieJar;
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;
        }
        public static string Delete(string ServerUrl, string dataPath)
        {
            Console.WriteLine(string.Format("{0}{1}", ServerUrl, dataPath));
            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", ServerUrl, dataPath));
            request.CookieContainer = CookieJar;
            request.Method = "DELETE";
            request.ContentType = "application/x-www-form-urlencoded";
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;
        }

        public static async Task<string> GetAsync(string ServerUrl, string dataPath)
        {
            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", ServerUrl, dataPath));
            request.CookieContainer = CookieJar;
            var response = await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);
            var responseString = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
            return responseString;
        }

        public static async Task<string> SendAsync(string ServerUrl, string dataPath, string json, string method)
        {
            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", ServerUrl, dataPath));
            request.CookieContainer = CookieJar;
            request.Method = method;
            request.ContentType = "application/json";
            request.ContentLength = json.Length;

            using (var stream = request.GetRequestStreamAsync())
            using (var writeStream = new StreamWriter(await stream))
            {
                await writeStream.WriteAsync(json);
            }
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

            var responseString = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
            return responseString;
        }

        public static async Task<string> DeleteAsync(string ServerUrl, string dataPath)
        {
            Console.WriteLine(string.Format("{0}{1}", ServerUrl, dataPath));
            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", ServerUrl, dataPath));
            request.CookieContainer = CookieJar;
            request.Method = "DELETE";
            request.ContentType = "application/x-www-form-urlencoded";
            var response = await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);
            var responseString = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
            return responseString;
        }

        static Ping Checker = new Ping();

        async public static Task<bool> CheckAliveAsync(string location)
        {
            try
            {
                Uri u = new Uri(location);
                PingReply reply = await Checker.SendPingAsync(u.Host);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}
