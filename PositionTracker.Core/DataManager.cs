using System;
using System.Threading;
using System.Threading.Tasks;
using PositionTracker.Core.Attributes;
using PositionTracker.Event;
using PositionTracker.Event.Events;
using PositionTracker.Utility;

namespace PositionTracker.Core
{
    public class DataManager : IExitsGracefully
    {
        public long LoopThreadInterval = Constant.RestApiTickerRefreshRate;
        private readonly ApiManager apiManager;

        private readonly Thread loopThread;
        private bool loopThreadAlive;

        public DataManager(ApiManager apiManager)
        {
            this.apiManager = apiManager;
            loopThread = new Thread(CoreLoop);
        }

        public void ExitGracefully()
        {
            loopThreadAlive = false;
        }

        public void Init()
        {
            loopThreadAlive = true;
            loopThread.Start();
        }

        [ExecutionTimeLog(10000)]
        private void CoreLoop()
        {
            while (loopThreadAlive)
            {
                try
                {
                    var e = LogExecutionTime.Begin();

                    var ops = apiManager.GetTickers().ContinueWith(task =>
                        EventManager.Instance.Fire(new UpdateSummaryEvent()));

                    Task.WaitAll(new[] {ops}, 60000);

                    var remaining = LoopThreadInterval - e.EndSilent();

                    if (remaining > 0) Thread.Sleep((int) remaining);
                }
                catch (Exception e)
                {
                    Logger.LogError($"CoreLoop Error", e);
                }
            }
        }
    }
}