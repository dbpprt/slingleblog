﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Microsoft.Owin;
using Microsoft.Owin.Diagnostics;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Practices.Unity;
using Owin;
using SlingleBlog.Common.Configuration;
using SlingleBlog.Common.Logging;
using SlingleBlog.Common.Unity;
using SlingleBlog.Common.UrlRewrite;
using SlingleBlog.Middlewares;

namespace SlingleBlog.Common
{
    public abstract class SlingleBootstrapper : IUnityModule
    {
        protected IUnityContainer UnityContainer
        {
            get { return _container; }
        }

        protected ILog Logger
        {
            get { return UnityContainer.Resolve<ILog>(); }
        }

        private readonly IUnityContainer _container;

        public SlingleBootstrapper(
            IUnityContainer container
            )
        {
            _container = container;
        }

        public SlingleBootstrapper()
            : this(new UnityContainer())
        { }

        public abstract void RegisterDependencies(IUnityContainer container);

        public virtual void Build(IAppBuilder app)
        {
            var errorPageOptions = ErrorPageOptions();
            if (errorPageOptions != null)
            {
                app.UseErrorPage(errorPageOptions);
            }

            _container.RegisterModule(this);

            _container.RegisterType<ILog, AggregateLog>(new HierarchicalLifetimeManager());

            var dependencyResolver = new UnityDependencyResolver(_container);

            app.SetApplicationContainer(dependencyResolver);
            app.Use(
                new Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>(
                    next => new ContainerMiddleware(next, app, ConfigureRequestContainer, ApplicationStartup).Invoke));

            var config = new HttpConfiguration
            {
                DependencyResolver = dependencyResolver,
                IncludeErrorDetailPolicy = IncludeErrorDetailPolicy(),
            };
            config.MapHttpAttributeRoutes();

            RegisterRoutes(config.Routes);
            ConfigureHttpConfiguration(config);

            app.UseWebApi(new OwinDependencyScopeHttpServerAdapter(config));

            var rewriteRules = new List<RewriteRule>();
            RegisterRewriteRules(rewriteRules);
            var rewriteHttpRoutes = new HttpRouteCollection();

            foreach (var rule in rewriteRules)
            {
                rewriteHttpRoutes.MapHttpRoute(
                    rule.Name,
                    rule.Template,
                    new { rewrite = rule.RewriteTo },
                    new { data = rule.Constraint }
                    );
            }
            app.Use<UrlRewriteMiddleware>(rewriteHttpRoutes);

            var staticFileOptions = StaticFileOptions();
            if (staticFileOptions != null)
            {
                app.UseStaticFiles(staticFileOptions);
            }

        }

        protected virtual void RegisterRoutes(HttpRouteCollection routes) { }

        protected virtual void RegisterRewriteRules(ICollection<RewriteRule> rules) { }

        protected virtual Task ApplicationStartup()
        {
            return Task.FromResult(0);
        }

        protected virtual void ConfigureHttpConfiguration(HttpConfiguration configuration) { }

        protected virtual Task ConfigureRequestContainer(IUnityContainer container, IOwinContext context)
        {
            return Task.FromResult(0);
        }

        protected virtual IncludeErrorDetailPolicy IncludeErrorDetailPolicy()
        {
            return System.Web.Http.IncludeErrorDetailPolicy.LocalOnly;
        }

        protected virtual ErrorPageOptions ErrorPageOptions()
        {
            return null;
        }

        protected virtual StaticFileOptions StaticFileOptions()
        {
            return null;
        }

        private class ContainerMiddleware
        {
            private readonly Func<IDictionary<string, object>, Task> _next;
            private readonly IAppBuilder _app;
            private readonly Func<IUnityContainer, IOwinContext, Task> _configureRequestContainer;
            private readonly Func<Task> _applicationStartup;
            private readonly SemaphoreSlim _applicationStartupLock = new SemaphoreSlim(1);
            private int _requests = 0;

            public ContainerMiddleware(
                Func<IDictionary<string, object>, Task> next,
                IAppBuilder app,
                Func<IUnityContainer, IOwinContext, Task> configureRequestContainer,
                Func<Task> applicationStartup)
            {
                _next = next;
                _app = app;
                _configureRequestContainer = configureRequestContainer;
                _applicationStartup = applicationStartup;
            }

            public async Task Invoke(IDictionary<string, object> environment)
            {
                if (_requests == 0)
                {
                    await _applicationStartupLock.WaitAsync();

                    if (_requests == 0)
                    {
                        try
                        {
                            await _applicationStartup();
                        }
                        finally
                        {
                            _requests++;
                            _applicationStartupLock.Release();
                        }
                    }
                }

                using (var scope = environment.SetRequestContainer(_app))
                {
                    _configureRequestContainer(scope.GetUnderlayingContainer(), new OwinContext(environment));
                    await _next.Invoke(environment);
                }
            }
        }

    }
}
