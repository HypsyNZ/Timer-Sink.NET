using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimerSink
{
    internal static class Sink
    {
        /// <summary>
        /// Task Task Task
        /// </summary>
        /// <param name="task">The Task</param>
        public static async void Task(this Task task, bool shouldAwait = false)
        {
            try
            {
                await task.ConfigureAwait(shouldAwait);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}