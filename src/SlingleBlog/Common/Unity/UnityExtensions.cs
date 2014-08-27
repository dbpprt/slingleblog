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
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;

namespace SlingleBlog.Common.Unity
{
    public static class UnityExtensions
    {
        public static IUnityContainer RegisterModule<TModule>(this IUnityContainer container) where TModule : IUnityModule
        {
            var module = Activator.CreateInstance<TModule>();
            module.RegisterDependencies(container);

            return container;
        }

        public static IUnityContainer RegisterModule(this IUnityContainer container, IUnityModule module) 
        {
            module.RegisterDependencies(container);

            return container;
        }

        public static IUnityContainer RegisterNamed<TFrom, TTo>(this IUnityContainer container, LifetimeManager lifetimeManager,
                                                               params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            Guard.ArgumentNotNull(container, "container");
            return container.RegisterType(typeof(TFrom), typeof(TTo), typeof(TTo).Name, lifetimeManager, injectionMembers);
        }

    }
}
