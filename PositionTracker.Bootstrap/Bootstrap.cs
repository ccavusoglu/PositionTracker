using Autofac;
using Autofac.Extras.DynamicProxy;
using PositionTracker.Core;
using PositionTracker.Core.LocalApi;
using PositionTracker.Domain.Entity;
using PositionTracker.Proxy.BinanceClient;
using PositionTracker.Utility;

namespace PositionTracker.Bootstrap
{
    public class Bootstrap
    {
        public static IContainer FacContainer { get; set; }

        public Bootstrap() { }

        public void Init()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<PerformanceInterceptor>().SingleInstance();
            builder.RegisterType<AvailableCoins>().SingleInstance();
            builder.RegisterType<BinanceRestClient>().SingleInstance();
            builder.RegisterType<DataManager>()
                .EnableClassInterceptors()
                .InterceptedBy(typeof(PerformanceInterceptor))
                .SingleInstance();
            builder.RegisterType<SecurityManager>().SingleInstance();
            builder.RegisterType<EntityManager>().SingleInstance();
            builder.RegisterType<LocalApiManager>().SingleInstance();
            builder.RegisterType<SocketApiManager>().SingleInstance();
            builder.RegisterType<ApiManager>()
                .EnableClassInterceptors()
                .InterceptedBy(typeof(PerformanceInterceptor))
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                .SingleInstance();
            builder.RegisterType<UserManager>()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                .SingleInstance();

            FacContainer = builder.Build();

            SetupLogger();
            SetupApiManagers();

            FacContainer.Resolve<UserManager>().Init();
            FacContainer.Resolve<DataManager>().Init();
        }

        private void SetupApiManagers()
        {
            var socketApiManager = FacContainer.Resolve<SocketApiManager>();
            var apiManager = FacContainer.Resolve<ApiManager>();

            socketApiManager.InitLocalServer();
            socketApiManager.Init();
            apiManager.Init();
        }

        public void SetupLogger() { }

        public void ExitGracefully()
        {
            Logger.FlushLogger();
        }
    }
}