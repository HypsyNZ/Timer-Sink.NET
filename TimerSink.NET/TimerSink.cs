//******************************************************************************************************
//  Copyright © 2022, S Christison. No Rights Reserved.
//
//  Licensed to [You] under one or more License Agreements.
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//
//******************************************************************************************************

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

        #endregion [Internal]
    }
}