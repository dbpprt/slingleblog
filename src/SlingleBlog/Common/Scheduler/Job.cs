using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace SlingleBlog.Common.Scheduler
{
    public abstract class Job
    {
        public abstract Guid Id { get; }

        public abstract string JobName { get; }

        public abstract string Description { get; }

        public abstract TimeSpan RunEvery { get; }

        public abstract Task Execute(CancellationToken cancellationToken, IUnityContainer scope);
    }
}
