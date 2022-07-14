/*
*MIT License
*
*Copyright (c) 2022 S Christison
*
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*
*The above copyright notice and this permission notice shall be included in all
*copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
*SOFTWARE.
*/

using PrecisionTiming;
using System;
using System.Threading.Tasks;

namespace TimerSink
{
    /// <summary>
    /// A Timing Sink Item that should be added to the <see cref="TimingSink.TimingSinkItems"/>
    /// <para>This allows a Mulitmedia Timer (PrecisionTimer) to be used effectively by ensuring that the event can't elapse more than once</para>
    /// </summary>
    public class TimingSinkItem
    {
        private Action action;
        private int interval;
        private bool shouldwait, throwOnError;

        /// <summary>
        /// A <see cref="TimingSinkItem"/> that can be consumed by a <see cref="TimingSink"/>
        /// </summary>
        /// <param name="action">The Action that will run as a <see cref="Task"/> when the Interval Elapses</param>
        /// <param name="interval">The amount of time between each Action in Milliseconds</param>
        /// <param name="taskShouldAwait">Whether the Task should wait for the UI context</param>
        /// <param name="throwOnError">True if exceptions should throw errors this may block your UI thread</param>
        public TimingSinkItem(Action action, int interval, bool taskShouldAwait = false, bool throwOnError = false)
        {
            this.Action = action;
            this.Interval = interval;
            this.TaskShouldAwait = taskShouldAwait;
            this.ThrowOnError = throwOnError;

            SinkItemTimer = new PrecisionTimer();
            SinkItemTimer.SetInterval(() => ItemTask(), 1, false);
        }

        #region [Public]

        /// <summary>
        /// The <see cref="Interval"/> between <see cref="Action"/>s
        /// <para>The amount of time between each Action in Milliseconds</para>
        /// </summary>
        public int Interval { get => interval; set => interval = value; }

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

        /// <summary>
        /// True if exceptions should throw errors this may block your UI thread
        /// </summary>
        public bool ThrowOnError { get => throwOnError; set => throwOnError = value; }

        #endregion [Public]

        #region [Internal]

        internal double LastRunTime = 0;
        internal readonly PrecisionTimer SinkItemTimer = new PrecisionTimer();

        internal bool ItemTimerRunning => SinkItemTimer.IsRunning() ?? false;

        internal void StartItemTask()
        { SinkItemTimer.Start(); }

        internal void StopItemTask()
        { SinkItemTimer.Stop(); LastRunTime = 0; }

        /// <summary>
        /// Runs the Action for this <see cref="TimingSinkItem"/> when the time has Elapsed
        /// </summary>
        /// <returns></returns>
        internal Task ItemTask()
        {
            if (TimingSink.CurrentTime >= LastRunTime + Interval)
            {
                LastRunTime = TimingSink.CurrentTime;

                _ = Task.Run(() =>
                {
                    try
                    {
                        Action();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString());

                        if (ThrowOnError) { throw ex; };
                    }
                }).ConfigureAwait(TaskShouldAwait);
            }

            return null;
        }

        #endregion [Internal]
    }
}
