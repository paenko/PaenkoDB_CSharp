using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PaenkoDB
{
    public class HealthCheck
    {
        static Timer _Timer;
        static Action<List<Node>> TimerElapsedCallback;
        static List<Node> TimerElapsedToCheck;

        /// <summary>
        /// Check the availability of a list of nodes
        /// </summary>
        /// <param name="toCheck">The node list that will be checked</param>
        /// <returns>All nodes that are available</returns>
        public async static Task<List<Node>> CheckHealth(List<Node> toCheck)
        {
            List<Node> Alive = new List<Node>();
            foreach (Node pn in toCheck)
            {
                if(await NetworkHandler.CheckAliveAsync(pn.NodeLocation.ip)) Alive.Add(pn);
            }
            return Alive;
        }
        /// <summary>
        /// Check node status in a specified interval
        /// </summary>
        /// <param name="toCheck">The node list that will be checked</param>
        /// <param name="callback">A callback that will fire after each interval. It has the available nodes as parameter</param>
        /// <param name="intervalInSeconds">The interval in wich the checks will be performed</param>
        public static void SetTimedCheck(List<Node> toCheck, Action<List<Node>> callback, int intervalInSeconds)
        {
            TimerElapsedCallback = callback;
            TimerElapsedToCheck = toCheck;
            _Timer = new Timer(Elapsed, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervalInSeconds));
        }

        static async void Elapsed(object state)
        {
            TimerElapsedCallback(await CheckHealth(TimerElapsedToCheck));
        }
    }
}
