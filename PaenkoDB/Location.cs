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
        public string country_name { get; set; }
        public string country_code { get; set; }
        public string city { get; set; }
        public string ip { get; set; }
        public int HttpPort { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }

        public static Location Lookup(string ip, int port)
        {
            string json = NetworkHandler.GET("http://api.hostip.info/get_json.php", $"?ip={ip}&position=true");
            Location _return = JsonConvert.DeserializeObject<Location>(json);
            _return.HttpPort = port;
            return _return;
        }

        public string HttpAddress()
        {
            return $"http://{ip}:{HttpPort}/";
        }
    }
}
