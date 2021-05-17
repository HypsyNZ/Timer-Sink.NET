using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimerSink
{
    public class TimingSinkItem
    {
        private Action action;
        private double lastrun;
        private double interval;
        private bool shouldwait;

        /// <summary>
        /// A <see cref="TimingSinkItem"/> that can be consumed by a <see cref="TimingSink"/>
        /// </summary>
        /// <param name="action">The Action that will run as a <see cref="Task"/> when the Interval Elapses</param>
        /// <param name="interval">The amount of time between each Action in Milliseconds</param>
        /// <param name="taskShouldAwait">Whether the Task should wait for the UI context</param>
        public TimingSinkItem(Action action, double interval, bool taskShouldAwait = false)
        {
            this.Action = action;
            this.Interval = interval;
            this.TaskShouldAwait = taskShouldAwait;
        }

        /// <summary>
        /// The <see cref="Interval"/> between <see cref="Action"/>s
        /// <para>The amount of time between each Action in Milliseconds</para>
        /// </summary>
        public double Interval { get => interval; set => interval = value; }

        /// <summary>
        /// This is measured in <see cref="SinkStopwatch"/> time which is the amount of time in milliseconds since <see cref="SinkTimerTask"/> was first called
        /// <para>The last time the Action Ran</para>
        /// </summary>
        public double LastRunTime { get => lastrun; set => lastrun = value; }

        /// <summary>
        /// The <see cref="Action"/> to run at the <see cref="Interval"/>
        /// <para>The Action that will run as a <see cref="Task"/> when the Interval Elapses</para>
        /// </summary>
        public Action Action { get => action; set => action = value; }

        /// <summary>
        /// Whether the Task should wait for the UI context
        /// <para>There are several things to consider if you do this</para>
        /// </summary>
        public bool TaskShouldAwait { get => shouldwait; set => shouldwait = value; }
    }
}