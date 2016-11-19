using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PaenkoDB
{
    public class PaenkoNode
    {
        public Location NodeLocation { get; set; }
        public PaenkoNode(string ip, int port)
        {
            NodeLocation = Location.Lookup(ip, port);
        }
    }
}
