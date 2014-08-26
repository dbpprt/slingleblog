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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Owin;

namespace SlingleBlog.Common.Unity
{
    public class ContainerMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _nextFunc;
        private readonly IAppBuilder _app;

        public ContainerMiddleware(Func<IDictionary<string, object>, Task> nextFunc, IAppBuilder app)
        {
            _nextFunc = nextFunc;
            _app = app;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            using (var scope = environment.SetRequestContainer(_app))
            {
                IEnumerable<IDynamicRegistrationDelegate> delegates = null;
                var context = new OwinContext(environment);

                try
                {
                    delegates = scope.GetUnderlayingContainer().ResolveAll<IDynamicRegistrationDelegate>();
                }
                catch { }

                try
                {
                    if (delegates != null)
                    {
                        delegates = delegates.ToList();
                        delegates.ForEach(_ => _.InterceptRequestScope(scope, context));
                    }

                    await _nextFunc(environment);
                }
                catch (Exception exception)
                {
                    throw;
                }
            }
        }
    }
}
