using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Diagnostics;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.StaticFiles;
using Microsoft.Practices.Unity;
using MobileDB.Common.Factory;
using MobileDB.Contracts;
using Nito.AsyncEx;
using SlingleBlog.Common.Configuration;
using SlingleBlog.Common.Framework;
using SlingleBlog.Common.PrerenderEngine;
using SlingleBlog.Common.Scheduler;
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

            var connectionString = ContextFactory.Create<EntityContext>()
                .WithPhysicalFilesystem(_configuration.StorageBasePath)
                .ConnectionString;
            container.RegisterType<IDbContext, EntityContext>(new HierarchicalLifetimeManager(),
                new InjectionConstructor(connectionString));
            container.RegisterType(typeof (IEntitySet<>),
                new InjectionFactory(EntitySetFactory));

            container.RegisterInstance(new AsyncReaderWriterLock());
        }

        private object EntitySetFactory(IUnityContainer unityContainer, Type type, string arg3)
        {
            var context = unityContainer.Resolve<IDbContext>();
            var entityType = type.GenericTypeArguments.First();
            var set = context.Set(entityType);
            return set;
        }

        protected override void RegisterJobs(List<Job> jobs)
        {
            base.RegisterJobs(jobs);

            jobs.Add(new PrerenderJob(_configuration.PrerendererSettings));
        }

        public override bool UseScheduler
        {
            get { return _configuration.UseScheduler; }
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

        protected override void RegisterRewriteRules(List<IRewriteRule> rules)
        {
            base.RegisterRewriteRules(rules);

            rules.AddRange(_configuration.RewriteRules);
        }

        protected override async Task ExecutePipeline(
            Func<IDictionary<string, object>, Task> next, 
            IDictionary<string, object> environment, 
            IUnityContainer scope)
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
            //configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
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

        protected override OAuthAuthorizationServerOptions OAuthAuthorizationServerOptions()
        {
            return new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/oauth/token"),
                Provider = new ApplicationOAuthProvider(_configuration.PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/oauth/external"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
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
