using Microsoft.Owin;
using Owin;
using SlingleBlog.Hosting.IIS;

[assembly: OwinStartup(typeof(Startup))]

namespace SlingleBlog.Hosting.IIS
{
    public class Startup
    {
        
        public void Configuration(IAppBuilder app)
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Build(app);
        }
    }
}
