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
using System.Linq;

namespace SlingleBlog.Common.Logging
{
    public static class ExceptionExtensions
    {
        public static Event ToEvent(this Exception exception)
        {
            return new Event(exception);
        }

        public static Exception Innermost(this Exception exception)
        {
            Exception result;
            for (result = exception; result.InnerException != null; result = result.InnerException) { }
            return result;
        }

        public static string Message(this Exception exception)
        {
            if (exception == null)
                return String.Empty;

            var aggregateException = exception as AggregateException;
            return aggregateException != null 
                ? String.Join(Environment.NewLine, aggregateException.InnerExceptions
                    .Where(inner => !String.IsNullOrEmpty(inner.Message))
                    .Select(inner => inner.Message)) 
                : exception.Message;
        }
    }
}
