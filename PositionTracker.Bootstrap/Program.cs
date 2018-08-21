using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using PositionTracker.Core;
using PositionTracker.Core.LocalApi;
using PositionTracker.Utility;

namespace PositionTracker.Bootstrap
{
    class Program
    {
        private static Bootstrap Bootstrap;

        static void Main(string[] args)
        {
//            Test();

            OnInit();

            EnterCommandLoop();

            OnExit();

#if DEBUG
            Console.ReadKey();
#endif
        }

        private static void OnExit()
        {
            var types = Bootstrap.FacContainer
                .ComponentRegistry
                .Registrations.Where(r => typeof(IExitsGracefully)
                    .IsAssignableFrom(r.Activator.LimitType)).Select(r => r.Activator.LimitType);

            foreach (var type in types)
            {
                IExitsGracefully impl;

                if (type.Namespace == "Castle.Proxies")
                    impl = Bootstrap.FacContainer.Resolve(type.BaseType) as IExitsGracefully;
                else
                    impl = Bootstrap.FacContainer.Resolve(type) as IExitsGracefully;

                // ReSharper disable once PossibleNullReferenceException
                // always expected not to be null
                impl.ExitGracefully();
            }

            Bootstrap.ExitGracefully();
        }

        private static void Test() { }

        private static void OnInit()
        {
            Console.WriteLine("Starting UI...");

            // start ui application.

            Bootstrap = new Bootstrap();

            try
            {
                Bootstrap.Init();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception while initializing: {e.Message} {e.StackTrace}");
            }

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                Logger.LogError("Task's uncaught exception.", args.Exception);
            };
        }

        private static void EnterCommandLoop()
        {
            var running = true;

            while (running)
            {
                var read = Console.ReadLine();

                if (string.IsNullOrEmpty(read)) continue;

                read = read.ToLowerInvariant();

                if (read.StartsWith("q"))
                {
                    Console.WriteLine("Quitting...");
                    break;
                }

                var socketApiManager = Bootstrap.FacContainer.Resolve<SocketApiManager>();
                var localApiManager = Bootstrap.FacContainer.Resolve<LocalApiManager>();
                var apiManager = Bootstrap.FacContainer.Resolve<ApiManager>();
                var userManager = Bootstrap.FacContainer.Resolve<UserManager>();

                if (read == "cls")
                    Console.Clear();
                else if (read == "save")
                    localApiManager.SaveUserData();
                else if (read == "fetchpositions")
                    Task.Factory.StartNew(async () => { await apiManager.FetchPositions(); });
                else if (read == "sendpositions")
                    localApiManager.SendPositions();
                else if (read == "gettickers")
                    Task.Factory.StartNew(async () => { await apiManager.GetTickers(); });
                else if (read == "sendtickers")
                    localApiManager.SendTickers();
                else if (read == "getall") Task.Factory.StartNew(() => apiManager.GetAvailableCoins());
                else if (read == "gccollect") GC.Collect();
            }
        }
    }
}