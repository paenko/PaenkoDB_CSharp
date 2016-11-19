using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaenkoDB
{
    public class Location
    {
        public string country { get; set; }
        public string countryCode { get; set; }
        public string region { get; set; }
        public string regionName { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public decimal lat { get; set; }
        public decimal lon { get; set; }
        public string timezone { get; set; }
        public string ip { get; set; }
        public string query { get; set; }
        public int HttpPort { get; set; }

        public static Location Lookup(string ip, int port)
        {
            string json = NetworkHandler.GET("http://ip-api.com/json/", ip);
            Location _return = JsonConvert.DeserializeObject<Location>(json);
            _return.HttpPort = port;
            _return.ip = _return.query;
            return _return;
        }

        public string HttpAddress()
        {
            return $"http://{ip}:{HttpPort}/";
        }
    }
}
