using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Owin.Hosting;
using Owin;
using SlingleBlog.Common.Configuration;

namespace SlingleBlog.Hosting.WindowsService
{
    internal class OwinHostService
    {
        private IDisposable _webApplication;

        public OwinHostService()
        {

        }

        public void Start()
        {
            var endpoint = "";


            _webApplication = WebApp.Start("http://+:8080/", Startup);
        }

        private void Startup(IAppBuilder appBuilder)
        {
#if DEBUG
            appBuilder.Use<ConsoleLoggerMiddleware>();
#endif

            var bootstrapper = new Bootstrapper();
            bootstrapper.Build(appBuilder);
        }

        public void Stop()
        {
            _webApplication.Dispose();
            _webApplication = null;
        }
    }
}