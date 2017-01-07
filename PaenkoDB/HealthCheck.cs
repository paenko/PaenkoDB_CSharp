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
        static Action<List<PaenkoNode>> TimerElapsedCallback;
        static List<PaenkoNode> TimerElapsedToCheck;

        public async static Task<List<PaenkoNode>> CheckHealth(List<PaenkoNode> toCheck)
        {
            List<PaenkoNode> Alive = new List<PaenkoNode>();
            foreach (PaenkoNode pn in toCheck)
            {
                if(await NetworkHandler.CheckAliveAsync(pn.NodeLocation.ip)) Alive.Add(pn);
            }
            return Alive;
        }

        public static void SetTimedCheck(List<PaenkoNode> toCheck, Action<List<PaenkoNode>> callback, int intervalInSeconds)
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
