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
        public string Location { get; set; }
        public UInt64 ServerID { get; set; }
        public string Description { get; set; }
        public PaenkoNode(string location, UInt64 serverID, string description)
        {
            Location = location;
            ServerID = serverID;
            Description = description;
        }
    }
}
