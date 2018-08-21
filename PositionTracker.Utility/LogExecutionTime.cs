using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PositionTracker.Utility
{
    public class LogExecutionTime
    {
        private readonly Stopwatch timer = new Stopwatch();

        private LogExecutionTime()
        {
            timer.Start();
        }

        public static LogExecutionTime Begin()
        {
            return new LogExecutionTime();
        }

        public void End([CallerFilePath] string callerClass = "", [CallerMemberName] string methodName = "",
            long threshold = 0,
            string customMessage = "")
        {
            var elapsedMilliseconds = timer.ElapsedMilliseconds;

            timer.Stop();

            if (elapsedMilliseconds >= threshold)
                Logger.LogPerf(
                    $"{customMessage}Executed In: {elapsedMilliseconds}ms Thread: {Thread.CurrentThread.ManagedThreadId}",
                    callerClass, methodName);
        }

        public long EndSilent()
        {
            var elapsedMilliseconds = timer.ElapsedMilliseconds;

            timer.Stop();

            return elapsedMilliseconds;
        }

        public long Interval()
        {
            var elapsedMilliseconds = timer.ElapsedMilliseconds;

            return elapsedMilliseconds;
        }

        public void LogInterval(string interval, [CallerFilePath] string callerClass = "",
            [CallerMemberName] string methodName = "")
        {
            var elapsedMilliseconds = timer.ElapsedMilliseconds;

            Logger.LogPerf($"{interval} at: {elapsedMilliseconds}ms", callerClass, methodName);
        }
    }
}