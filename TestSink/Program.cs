using System;
using System.Threading.Tasks;
using TimerSink;

namespace TestSink
{
    internal class Program
    {
        const int SINK_EVENT_TIME_MS = 999;
        static readonly TimingSink tSink = new TimingSink();

        private static void TaskMethodOne()
        {
            Console.WriteLine("One: Firing This Event Every " + SINK_EVENT_TIME_MS + "ms");
        }

        private static void TaskMethodTwo()
        {
            Console.WriteLine("Two: Firing This Event Every " + SINK_EVENT_TIME_MS + "ms");
            Console.WriteLine("Sink Faulted: " + tSink.SinkFaulted); // False
        }

        static void Main()
        {

            TimingSinkItem TaskToTime = new TimingSinkItem(TaskMethodOne, SINK_EVENT_TIME_MS);
            TimingSinkItem TaskToTimeTwo = new TimingSinkItem(TaskMethodTwo, SINK_EVENT_TIME_MS);

            tSink.TimingSinkItems.Add(TaskToTime);
            tSink.TimingSinkItems.Add(TaskToTimeTwo);

            _ = SinkTest().ConfigureAwait(false);
            _ = SinkHealth().ConfigureAwait(false);

            Console.ReadLine();
        }

        static async Task SinkTest()
        {
            while (true)
            {
                Console.WriteLine("Starting Sink and waiting 5 Seconds");
                tSink.Start();

                Console.WriteLine("Sink Faulted: " + tSink.SinkFaulted); // False

                // 10 Results (2 of each Task)
                await Task.Delay(5000).ConfigureAwait(false);

                Console.WriteLine("Stopping Sink and waiting 5 Seconds");
                tSink.Stop();

                Console.WriteLine("Sink Faulted: " + tSink.SinkFaulted); // True

                // 10 Results (2 of each Task)
                await Task.Delay(5000).ConfigureAwait(false);
            }
        }
        static async Task SinkHealth()
        {
            while (true)
            {
                await Task.Delay(2000).ConfigureAwait(false);
                Console.WriteLine("Sink Healthy: " + (tSink.SinkFaulted ? "No" : "Yes"));
            }
        }
    }
}
