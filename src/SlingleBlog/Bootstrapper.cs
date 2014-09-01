using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FileBiggy.Contracts;
using FileBiggy.Factory;
using FileBiggy.IoC;
using Microsoft.Owin;
using Microsoft.Owin.Diagnostics;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Practices.Unity;
using Newtonsoft.Json.Serialization;
using Nito.AsyncEx;
using OpenQA.Selenium.PhantomJS;
using SlingleBlog.Common.Configuration;
using SlingleBlog.Common.Framework;
using SlingleBlog.DataAccess;

namespace SlingleBlog
{
    public class Bootstrapper : SlingleBootstrapper
    {
        private readonly IConfiguration _configuration;
        private readonly AsyncReaderWriterLock _lock;

        public const string ConfigFile = "config.json";

        public Bootstrapper(IConfiguration configuration)
        {
            _configuration = configuration;
            _lock = new AsyncReaderWriterLock();
        }

        public Bootstrapper()
            : this(
                JsonConfiguration.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFile))
                ) { }

        public override void RegisterDependencies(IUnityContainer container)
        {
            container.RegisterInstance<IConfiguration>(_configuration);

            var entityContext = ContextFactory.Create<EntityContext>()
                .AsJsonDatabase()
                .WithDatabaseDirectory(_configuration.StorageBasePath)
                .Build();
            container.RegisterInstance<IBiggyContext>(entityContext);
            container.RegisterType(typeof(IEntitySet<>), typeof(Repository<>), new ContainerControlledLifetimeManager());

            container.RegisterInstance(new AsyncReaderWriterLock());
            container.RegisterInstance(PhantomJSDriverService.CreateDefaultService());
            container.RegisterType<PhantomJSDriver>(new TransientLifetimeManager(), new InjectionFactory(
                unityContainer => new PhantomJSDriver(unityContainer.Resolve<PhantomJSDriverService>())));
        }

        protected override Task ApplicationStartup()
        {
            Logger.Write("Server started successfully and is ready to receive requests!")
                .IsVerbose()
                .Save();

            Task.Factory.StartNew(() =>
            {
                using (var engine = UnityContainer.Resolve<PhantomJSDriver>())
                {
                    var manage = engine.Manage();
                    manage.Window.Maximize();

                    engine.Url = "http://localhost:8080/";
                    engine.Navigate();
                    Thread.Sleep(2000);
                    engine.GetScreenshot().SaveAsFile("C:\\slingle\\" + Guid.NewGuid() + ".png", ImageFormat.Png);
                }
                return Task.FromResult(0);
            });

            return base.ApplicationStartup();
        }

        protected override void RegisterRoutes(HttpRouteCollection routes)
        {
            base.RegisterRoutes(routes);


        }

        protected override void RegisterRewriteRules(List<IRewriteRule> rules)
        {
            base.RegisterRewriteRules(rules);

            rules.AddRange(_configuration.RewriteRules);
        }

        protected override async Task ExecutePipeline(Func<IDictionary<string, object>, Task> next, IDictionary<string, object> environment, IUnityContainer scope)
        {
            using (var upgradable = await _lock.UpgradeableReaderLockAsync())
            {
                scope.RegisterInstance(upgradable);

                await base.ExecutePipeline(next, environment, scope);
            }
        }

        protected override void ConfigureHttpConfiguration(HttpConfiguration configuration)
        {
            base.ConfigureHttpConfiguration(configuration);
            configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        protected override ErrorPageOptions ErrorPageOptions()
        {
            return new ErrorPageOptions
            {
                ShowCookies = true,
                ShowEnvironment = true,
                ShowExceptionDetails = true,
                ShowHeaders = true,
                ShowQuery = true,
                ShowSourceCode = true,
                SourceCodeLineCount = 20
            };
        }

        protected override IncludeErrorDetailPolicy IncludeErrorDetailPolicy()
        {
            return System.Web.Http.IncludeErrorDetailPolicy.Always;
        }

        protected override FileServerOptions FileServerOptions()
        {
            return new FileServerOptions
            {
                FileSystem = new PhysicalFileSystem(_configuration.PublicPath),
                RequestPath = new PathString(""),
                EnableDefaultFiles = true,
            };
        }
    }
}
