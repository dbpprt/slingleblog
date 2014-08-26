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

using System.Linq;
using System.Threading.Tasks;

namespace SlingleBlog.Common.Logging
{
    public class AggregateLog : ILog
    {
        private readonly ILogAdapter[] _adapters;

        public AggregateLog(
            ILogAdapter[] adapters
            )
        {
            _adapters = adapters;
        }

        public Event Write()
        {
            return new Event(this);
        }

        public Event Write(string message)
        {
            return new Event(this)
                .SetMessage(message);
        }

        public void Save(Event data)
        {
            foreach (var adapter in _adapters)
            {
                adapter.Save(data);
            }
        }

        public Task SaveAsync(Event data)
        {
            var tasks = _adapters
                .Select(adapter => adapter.SaveAsync(data));

            return Task.WhenAll(tasks);
        }
    }
}
