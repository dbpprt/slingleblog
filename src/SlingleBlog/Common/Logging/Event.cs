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

namespace SlingleBlog.Common.Logging
{
    public class Event
    {
        private readonly ILog _underlayingLogger;

        public Guid CorrelationId { get; private set; }
        public LogLevel Level { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }
        public List<object> Objects { get; private set; }

        public Event(
            Exception exception
            ) : this()
        {
            Exception = exception;
        }

        public Event(
            ILog underlayingLogger
            ) : this()
        {
            _underlayingLogger = underlayingLogger;
        }

        public Event()
        {
            CorrelationId = Guid.NewGuid();
            Objects = new List<object>();
            Level = LogLevel.Warning;
        }

        public Event IsAudit()
        {
            Level = LogLevel.Audit;
            return this;
        }

        public Event IsFailureAudit()
        {
            Level = LogLevel.FailureAudit;
            return this;
        }

        public Event IsCritical()
        {
            Level = LogLevel.Critical;
            return this;
        }

        public Event IsVerbose()
        {
            Level = LogLevel.Verbose;
            return this;
        }

        public Event IsError()
        {
            Level = LogLevel.Error;
            return this;
        }

        public Event IsInformation()
        {
            Level = LogLevel.Information;
            return this;
        }

        public Event IsUndefined()
        {
            Level = LogLevel.Undefined;
            return this;
        }

        public Event IsWarning()
        {
            Level = LogLevel.Warning;
            return this;
        }

        public Event SetMessage(string message)
        {
            Message = message;
            return this;
        }

        public Event WithObject(object obj)
        {
            Objects.Add(obj);
            return this;
        }

        public void Save(ILog log)
        {
            log.Save(this);
        }

        public Task SaveAsync(ILog log)
        {
            return log.SaveAsync(this);
        }

        public void Save()
        {
            if (_underlayingLogger == null)
            {
                throw new ArgumentNullException("Use this function only if used with ILog fluent API, otherwise call Save (ILog log)");
            }

            _underlayingLogger.Save(this);
        }

        public Task SaveAsync()
        {
            if (_underlayingLogger == null)
            {
                throw new ArgumentNullException("Use this function only if used with ILog fluent API, otherwise call Save (ILog log)");
            }

            return _underlayingLogger.SaveAsync(this);
        }
    }
}
