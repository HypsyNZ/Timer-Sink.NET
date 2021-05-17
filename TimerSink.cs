using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TimerSink
{
    public class TimingSink
    {
        private double currentTime;
        private Collection<TimingSinkItem> sinkItems = new Collection<TimingSinkItem>();

        /// <summary>
        /// The Stopwatch for this instance of TimingSink, Used to time the interval between Actions/Methods
        /// </summary>
        private readonly Stopwatch SinkStopwatch = new Stopwatch();

        /// <summary>
        /// The default collection for the current instance of TimingSink
        /// <para>Add <see cref="TimingSinkItem"/>s to this and then call <see cref="SinkTimerTask"/></para>
        /// </summary>
        public Collection<TimingSinkItem> TimingSinkItems
        {
            get => sinkItems;
            set => sinkItems = value;
        }

        /// <summary>
        /// This is the main task for the current <see cref="TimingSink"/> which uses the built in <see cref="TimingSinkItems"/>
        /// <para>You need to connect this to a <see cref="PrecisionTimer"/> or Timer of your choosing</para>
        /// <para>You should set this to run at 1ms intervals, It uses a <see cref="Stopwatch"/> interally to trigger <see cref="TimingSinkItem"/>s at the interval</para>
        /// <para>Each <see cref="TimingSink"/> should have its own Timer, You shouldn't put more than 10 <see cref="TimingSinkItem"/> in one <see cref="TimingSink"/></para>
        /// <para>Its guaranteed to be as accurate as your platform down to 1ms, If you use a timer that elapses faster than this (ideal) updates will be pinned to 1ms with greater precision</para>
        /// <para>The <see cref="SinkStopwatch"/> is started the first time you call <see cref="SinkTimerTask"/> you can't stop or reset it, its good for 100 years</para>
        /// </summary>
        public void SinkTimerTask()
        {
            SinkTimerTask(TimingSinkItems);
        }

        /// <summary>
        /// If you calculate that you have extra time or you want to use multiple sets of <see cref="TimingSinkItem"/> you can use <see cref="SinkTimerTask"/> directly.
        /// <para>You will have to supply a timer like when you call SinkTimerTask()</para>
        /// <para>Keep in mind that the interval will be compared to the current Instance of SinkStopWatch so it will fire instantly the first time if the Interval has already passed.</para>
        /// <para>It would be better to create a whole new <see cref="TimingSink"/> in most cases.</para>
        /// </summary>
        /// <param name="TimingSinkItems"></param>
        public void SinkTimerTask(Collection<TimingSinkItem> TimingSinkItems)
        {
            if (!SinkStopwatch.IsRunning) { try { SinkStopwatch.Restart(); } catch { } }

            // !You could make this more precise by comparing Ticks instead of Milliseconds but it will be unreliable under 0.5ms!
            // !It will also destroy your CPU!.
            // The good thing about using TotalMilliseconds is it fallsback automatically. (should always be available)
            currentTime = SinkStopwatch.Elapsed.TotalMilliseconds;

            // Check each Item
            foreach (TimingSinkItem s in TimingSinkItems)
            {
                // Elapsing or Already Elapsed
                if (currentTime >= s.LastRunTime + s.Interval)
                {
                    // Update Last Run Time BEFORE we run the Items Action
                    s.LastRunTime = currentTime;

                    // Run a Sink Task that will still catch errors
                    Sink.Task(SinkTimerAction(s), s.TaskShouldAwait);

                    return;
                }
            }
            return;
        }

        private Task SinkTimerAction(TimingSinkItem s)
        {
            s.Action();
            return Task.CompletedTask;
        }
    }
}