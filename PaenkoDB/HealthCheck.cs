using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaenkoDB
{
    public class HealthCheck
    {
        public async static Task<List<PaenkoNode>> CheckHealth(List<PaenkoNode> toCheck)
        {
            List<PaenkoNode> Alive = new List<PaenkoNode>();
            foreach (PaenkoNode pn in toCheck)
            {
                if(await NetworkHandler.CheckAliveAsync(pn.NodeLocation.ip)) Alive.Add(pn);
            }
            return Alive;
        }
    }
}
