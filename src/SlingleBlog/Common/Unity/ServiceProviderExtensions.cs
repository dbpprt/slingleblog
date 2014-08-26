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
using System.Web.Http.Dependencies;

namespace SlingleBlog.Common.Unity
{
    public static class ServiceProviderExtensions
    {
        public static T Resolve<T>(this IServiceProvider provider)
        {
            return (T) provider.GetService(typeof (T));
        }

        public static IEnumerable<T> ResolveAll<T>(this IDependencyResolver provider)
        {
            return provider.GetServices(typeof(T)).OfType<T>();
        }

        public static T Resolve<T>(this IDependencyResolver provider)
        {
            return (T)provider.GetService(typeof(T));
        }

        public static T Resolve<T>(this IDependencyScope provider)
        {
            return (T)provider.GetService(typeof(T));
        }

        public static IEnumerable<T> ResolveAll<T>(this IDependencyScope provider)
        {
            return provider.GetServices(typeof(T)).OfType<T>();
        }
    }
}
