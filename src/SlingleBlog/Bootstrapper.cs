using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Diagnostics;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Practices.Unity;
using Owin;
using SlingleBlog.Common;
using SlingleBlog.Common.Configuration;
using SlingleBlog.Common.Logging;
using SlingleBlog.Common.Unity;

namespace SlingleBlog
{
    public class Bootstrapper : SlingleBootstrapper
    {
        private readonly IConfiguration _configuration;
        public const string ConfigFile = "config.json";

        public Bootstrapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Bootstrapper() : this(
            JsonConfiguration.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFile))
            ) { }

        public override void RegisterDependencies(IUnityContainer container)
        {
            container.RegisterInstance<IConfiguration>(_configuration);
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
