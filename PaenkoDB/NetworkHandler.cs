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
        public static string Send(string ServerUrl, string dataPath, string json, string method)
        {
            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", ServerUrl, dataPath));
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

        public static string GET(string ServerUrl, string dataPath)
        {
            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", ServerUrl, dataPath));
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;
        }

        public static string DELETE(string ServerUrl, string dataPath)
        {
            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", ServerUrl, dataPath));
            request.Method = "DELETE";
            request.ContentType = "application/x-www-form-urlencoded";
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;
        }

        static Ping Checker = new Ping();

        public static bool CheckAlive(string location)
        {
            try
            {
                Uri u = new Uri(location);
                PingReply reply = Checker.Send(u.Host);
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
