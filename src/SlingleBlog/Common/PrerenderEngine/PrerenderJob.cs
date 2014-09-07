using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using SlingleBlog.Common.Configuration;
using SlingleBlog.Common.Scheduler;

namespace SlingleBlog.Common.PrerenderEngine
{
    class PrerenderJob : Job
    {
        private readonly IPrerendererSettings _configuration;

        public override Guid Id
        {
            get { return new Guid("8EF5CA4E-030A-42CF-B2FC-EEFE9BA9C62E"); } 
        }

        public override string JobName
        {
            get { return "prerenderer"; }
        }

        public override string Description
        {
            get { return "Precompiles the entire webapplication for google bot and other non-javascript targets"; }
        }

        public override TimeSpan RunEvery
        {
            get
            {
                return new TimeSpan(0, 0, 10);
            }
        }

        public PrerenderJob(
            IPrerendererSettings configuration
            )
        {
            _configuration = configuration;
        }

        public override Task Execute(CancellationToken cancellationToken, IUnityContainer scope)
        {
            scope.RegisterInstance(_configuration);
            var renderer = scope.Resolve<Prerenderer>();
            
            scope.RegisterInstance(renderer); // this should ensure that the renderer gets disposed from the unitycontainer!

            return renderer.ProcessPendingPages(cancellationToken);
        }
    }
}
