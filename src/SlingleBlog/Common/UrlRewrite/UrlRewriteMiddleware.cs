using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.Owin;

namespace SlingleBlog.Middlewares
{
    public class UrlRewriteMiddleware : OwinMiddleware
    {
        private readonly HttpRouteCollection _routes;

        public UrlRewriteMiddleware(OwinMiddleware next, HttpRouteCollection routes)
            : base(next)
        {
            _routes = routes;
        }

        public override Task Invoke(IOwinContext context)
        {
            var method = new HttpMethod(context.Request.Method);
            var requestMessage = new HttpRequestMessage(method, context.Request.Uri);

            var match = _routes.GetRouteData(requestMessage);

            ProcessMatch(match, context);

            return Next.Invoke(context);
        }

        private static bool ProcessMatch(IHttpRouteData match, IOwinContext context)
        {
            object value;
            if (match != null && match.Route.Defaults.TryGetValue("rewrite", out value))
            {
                var rewritePath = value as string;

                if (rewritePath != null)
                {
                    context.Request.Path = new PathString(rewritePath);
                }

                return false;
            }

            return true;
        }
    }
}
