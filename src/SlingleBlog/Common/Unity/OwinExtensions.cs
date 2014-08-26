#region Copyright (C) 2014 Applified.NET 
// Copyright (C) 2014 Applified.NET
// http://www.applified.net

// This file is part of Applified.NET.

// Applified.NET is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.

// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Owin;

namespace SlingleBlog.Common.Unity
{
    public static class OwinExtensions
    {
        public static IAppBuilder UseContainer(this IAppBuilder app, IDependencyResolver appContainer)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            if (appContainer == null)
            {
                throw new ArgumentNullException("appContainer");
            }

            SetApplicationContainer(app, appContainer);

            return app.Use(new Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>(nextApp => new ContainerMiddleware(nextApp, app).Invoke));
        }

        public static HttpServer PrepareWebapiAdapter(this IAppBuilder app, HttpConfiguration configuration)
        {
            var appContainer = GetApplicationContainer(app);
            configuration.DependencyResolver = appContainer;
            var httpServer = new OwinDependencyScopeHttpServerAdapter(configuration);
            return httpServer;
        }

        public static IAppBuilder SetApplicationContainer(this IAppBuilder app, IDependencyResolver container)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            app.Properties[Constants.OwinApplicationContainerKey] = container;

            return app;
        }

        public static IDependencyResolver GetApplicationContainer(this IAppBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            return app.Properties[Constants.OwinApplicationContainerKey] as IDependencyResolver;
        }
    }
}
