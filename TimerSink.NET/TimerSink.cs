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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TimerSink
{
    /// <summary>
    /// The Timing Sink Instance
    /// <para>Manages a pool of <see cref="TimingSinkItems"/></para>
    /// </summary>
    public class TimingSink
    {
        private Collection<TimingSinkItem> timingSinkItems = new Collection<TimingSinkItem>();

        #region [Public]

        /// <summary>
        /// The Collection of <see cref="TimingSinkItem"/>s that will run in this instance of <see cref="TimingSink"/>
        /// </summary>
        public Collection<TimingSinkItem> TimingSinkItems
        {
            get => timingSinkItems;
            set => timingSinkItems = value;
        }

        /// <summary>
        /// Check the Status of this Sink to see if it is running in an Exceptional State
        /// </summary>
        public bool SinkFaulted => !(SinkTimer.IsRunning() ?? false) || !CheckItemTimers();

        /// <summary>
        /// <para>Starts the Sink</para>
        /// <para>Starts the <see cref="TimingSinkItem"/>s and <see cref="SinkTimer"/></para>
        /// <para>Starts and Resets the <see cref="SinkStopwatch"/></para>
        /// </summary>
        public void Start()
        {
            if (!SinkStopwatch.IsRunning)
            {
                try
                {
                    foreach (var Item in TimingSinkItems)
                    {
                        Item.StartItemTask();
                    }

                    SinkTimer = new PrecisionTimer();

                    // To ensure all Tasks start running at the same time the SinkStopwatch needs to restart here
                    SinkStopwatch?.Restart();

                    // Events can't fire until UpdateStopWatch starts running
                    SinkTimer.SetInterval(() => UpdateStopWatch(), 1);
                }
                catch { }
            }
        }

        /// <summary>
        /// <para>Stops the Sink</para>
        /// <para>Stops the <see cref="TimingSinkItem"/>s and <see cref="SinkTimer"/></para>
        /// <para>Stops and Resets the <see cref="SinkStopwatch"/></para>
        /// </summary>
        public void Stop()
        {
            try
            {
                foreach (var Item in TimingSinkItems)
                {
                    Item.StopItemTask();
                }

                SinkTimer?.Stop();
                SinkStopwatch?.Reset();
                SinkStopwatch?.Stop();
            }
            catch { }
        }

        #endregion [Public]

        #region [Internal]

        internal static double CurrentTime = 0;
        internal readonly Stopwatch SinkStopwatch = new Stopwatch();
        internal PrecisionTimer SinkTimer = new PrecisionTimer();

        /// <summary>
        /// Updates the CurrentTime according to the SinkStopwatch
        /// <para>Timing of <see cref="TimingSinkItem"/>s is dependant on this <see cref="Stopwatch"/></para>
        /// </summary>
        /// <returns></returns>
        internal Task UpdateStopWatch()
        {
            CurrentTime = SinkStopwatch.ElapsedMilliseconds;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Enumerate Sink Items and check if they report that they are still running
        /// </summary>
        /// <returns></returns>
        internal bool CheckItemTimers()
        {
            bool okay = true;

            foreach (TimingSinkItem item in timingSinkItems)
            {
                if (!item.ItemTimerRunning)
                {
                    okay = false;
                }
            }

            return okay;
        }

        #endregion [Internal]
    }
}
