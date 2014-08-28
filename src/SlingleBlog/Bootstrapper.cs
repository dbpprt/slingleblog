using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
using Owin;
using SlingleBlog.Common;
using SlingleBlog.Common.Configuration;
using SlingleBlog.Common.Framework;
using SlingleBlog.Common.Logging;
using SlingleBlog.Common.Unity;
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

        public Bootstrapper() : this(
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
            container.RegisterType(typeof (IEntitySet<>), typeof (Repository<>), new ContainerControlledLifetimeManager());

            container.RegisterInstance(new AsyncReaderWriterLock());
        }

        protected override Task ApplicationStartup()
        {
            Logger.Write("Server started successfully and is ready to receive requests!")
                .IsVerbose()
                .Save();

            return base.ApplicationStartup();
        }

        protected override void RegisterRoutes(HttpRouteCollection routes)
        {
            base.RegisterRoutes(routes);


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

        protected override StaticFileOptions StaticFileOptions()
        {
            return new StaticFileOptions
            {
                FileSystem = new PhysicalFileSystem(_configuration.PublicPath),
                RequestPath = new PathString("")
            };
        }
    }
}
