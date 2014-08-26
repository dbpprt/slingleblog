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
using System.ComponentModel;
using System.Web.Http.Dependencies;
using Microsoft.Owin;
using Owin;

namespace SlingleBlog.Common.Unity
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class OwinEnvironmentExtensions
    {
        public static IUnityServiceProvider SetRequestContainer(this IDictionary<string, object> environment, IAppBuilder app)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            var appContainer = OwinExtensions.GetApplicationContainer(app);
            if (appContainer == null)
            {
                throw new InvalidOperationException("There is no application container registered to resolve a request container");
            }

            var requestContainer = appContainer.BeginScope();

            environment[Constants.OwinRequestContainerEnvironmentKey] = requestContainer;

            return requestContainer as IUnityServiceProvider;
        }

        public static IDependencyScope GetRequestContainer(
            this IDictionary<string, object> environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            var unityScope =
                environment[Constants.OwinRequestContainerEnvironmentKey] as IDependencyScope;

            return unityScope;
        }

        public static IDependencyScope GetRequestContainer(
            this IOwinContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var environment = context.Environment;

            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            var unityScope =
                environment[Constants.OwinRequestContainerEnvironmentKey] as IDependencyScope;

            return unityScope;
        }
    }
}
