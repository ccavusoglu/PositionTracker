using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PositionTracker.Core;
using PositionTracker.Domain.Entity;
using PositionTracker.Proxy.BinanceClient;
using PositionTracker.Utility;
using PositionTracker.WebUI.Controllers;
using PositionTracker.WebUI.Hub;

namespace PositionTracker.WebUI
{
    public class Startup
    {
        private IContainer container;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else { app.UseExceptionHandler("/Main/Error"); }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Main}/{action=Index}");
            });

            app.UseSignalR(routes => { routes.MapHub<MainHub>("/hub"); });

            applicationLifetime.ApplicationStopped.Register(OnApplicationStopped);
            applicationLifetime.ApplicationStarted.Register(OnApplicationStarted);

            BootStrap();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddMvc();

            var builder = new ContainerBuilder();
            builder.Populate(services);

            var types = new List<Type>
            {
                typeof(PositionController)
            };

            container = RegisterTypes(builder, types);

            return new AutofacServiceProvider(container);
        }

        private void BootStrap()
        {
            var culture = (CultureInfo) CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.CurrencyDecimalSeparator = ".";
            culture.NumberFormat.NumberDecimalSeparator = ".";
            culture.DateTimeFormat = DateTimeFormatInfo.InvariantInfo;

            Thread.CurrentThread.CurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            try
            {
                container.Resolve<UserManager>().Init();
                container.Resolve<ApiManager>().Init();
                container.Resolve<DataManager>().Init();
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

        private void OnApplicationStarted()
        {
            Logger.LogTrace("Application Started!");
        }

        private void OnApplicationStopped()
        {
            Logger.LogTrace("Application Stopped!");

            // stop workers and persist data.
            OnExit();
        }

        private void OnExit()
        {
            var types = container
                .ComponentRegistry
                .Registrations.Where(r => typeof(IExitsGracefully)
                    .IsAssignableFrom(r.Activator.LimitType)).Select(r => r.Activator.LimitType);

            foreach (var type in types)
            {
                IExitsGracefully impl;

                // TODO: is there a better way to find out if resolved object is a proxy?
                if (type.Namespace == "Castle.Proxies")
                    impl = container.Resolve(type.BaseType) as IExitsGracefully;
                else
                    impl = container.Resolve(type) as IExitsGracefully;

                impl?.ExitGracefully();
            }

            Logger.FlushLogger();
        }

        private IContainer RegisterTypes(ContainerBuilder builder, IList<Type> types)
        {
            builder.RegisterType<PerformanceInterceptor>().SingleInstance();
            builder.RegisterType<AvailableCoins>().SingleInstance();
            builder.RegisterType<BinanceRestClient>().SingleInstance();
            builder.RegisterType<DataManager>()
                .EnableClassInterceptors()
                .InterceptedBy(typeof(PerformanceInterceptor))
                .SingleInstance();
            builder.RegisterType<EntityManager>().SingleInstance();
            builder.RegisterType<ApiManager>()
                .EnableClassInterceptors()
                .InterceptedBy(typeof(PerformanceInterceptor))
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                .SingleInstance();
            builder.RegisterType<UserManager>()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                .SingleInstance();

            foreach (var type in types) { builder.RegisterType(type).SingleInstance(); }

            return builder.Build();
        }
    }
}