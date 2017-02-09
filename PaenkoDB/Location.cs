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
        public string country { get; set; } = "UNKOWN";
        public string countryCode { get; set; } = "UNKOWN";
        public string region { get; set; } = "UNKOWN";
        public string regionName { get; set; } = "UNKOWN";
        public string city { get; set; } = "UNKOWN";
        public string zip { get; set; } = "UNKOWN";
        public double lat { get; set; } = 0;
        public double lon { get; set; } = 0;
        public string timezone { get; set; } = "UNKOWN";
        public string ip { get; set; }
        public string query { get; set; } = "UNUSED";
        public int HttpPort { get; set; }

        public static Location Lookup(string ip, int port)
        {
            Location _return;
            string json = NetworkHandler.Get("http://ip-api.com/json/", ip);
            _return = JsonConvert.DeserializeObject<Location>(json);
            _return.HttpPort = port;
            _return.ip = _return.query;
            return _return;
        }

        async public static Task<Location> LookupAsync(string ip, int port)
        {
            string json = await NetworkHandler.GetAsync("http://ip-api.com/json/", ip);
            Location _return = await Task.Factory.StartNew(()=> JsonConvert.DeserializeObject<Location>(json));
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
