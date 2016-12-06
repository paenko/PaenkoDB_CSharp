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
        public double lat { get; set; }
        public double lon { get; set; }
        public string timezone { get; set; }
        public string ip { get; set; }
        public string query { get; set; }
        public int HttpPort { get; set; }

        public static Location Lookup(string ip, int port)
        {
            string json = NetworkHandler.Get("http://ip-api.com/json/", ip);
            Location _return = JsonConvert.DeserializeObject<Location>(json);
            _return.HttpPort = port;
            _return.ip = _return.query;
            if (_return.lat == 0 && _return.lon == 0) _return.country = _return.countryCode = _return.city = _return.region = _return.regionName = _return.timezone = _return.zip = "UNKOWN";
            return _return;
        }

        async public static Task<Location> LookupAsync(string ip, int port)
        {
            string json = await NetworkHandler.GetAsync("http://ip-api.com/json/", ip);
            Location _return = await Task.Factory.StartNew(()=> JsonConvert.DeserializeObject<Location>(json));
            _return.HttpPort = port;
            _return.ip = _return.query;
            if (_return.lat == 0 && _return.lon == 0) _return.country = _return.countryCode = _return.city = _return.region = _return.regionName = _return.timezone = _return.zip = "UNKOWN";
            return _return;
        }

        public string HttpAddress()
        {
            return $"http://{ip}:{HttpPort}/";
        }
    }
}
