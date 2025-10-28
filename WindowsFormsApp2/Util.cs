using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    internal class Util
    {
        private Dictionary<string, Stopwatch> timers = new Dictionary<string, Stopwatch>();
        private Dictionary<string, DateTime> startTimes = new Dictionary<string, DateTime>();

        public void Start(string timerName)
        {
            if (!timers.ContainsKey(timerName))
            {
                timers[timerName] = new Stopwatch();
                startTimes[timerName] = DateTime.Now;
            }
            timers[timerName].Start();
        }

        public TimeSpan Lap(string timerName)
        {
            if (timers.TryGetValue(timerName, out Stopwatch timer))
            {
                return timer.Elapsed;
            }
            throw new ArgumentException($"Timer '{timerName}' not found.");
        }

        public TimeSpan Stop(string timerName)
        {
            if (timers.TryGetValue(timerName, out Stopwatch timer))
            {
                timer.Stop();
                TimeSpan elapsed = timer.Elapsed;
                DateTime endTime = DateTime.Now;

                Console.WriteLine($"Timer: {timerName}");
                Console.WriteLine($"Start Time: {startTimes[timerName]:yyyy-MM-dd HH:mm:ss.fff}");
                Console.WriteLine($"End Time: {endTime:yyyy-MM-dd HH:mm:ss.fff}");
                Console.WriteLine($"Elapsed Time: {elapsed.TotalMilliseconds} ms");

                timers.Remove(timerName);
                startTimes.Remove(timerName);

                return elapsed;
            }
            throw new ArgumentException($"Timer '{timerName}' not found.");
        }
    }
}
