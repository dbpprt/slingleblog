using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace SlingleBlog.Hosting.WindowsService
{
    class ConsoleLoggerMiddleware : OwinMiddleware
    {
        public ConsoleLoggerMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            var path = context.Request.Path;
            var stopWatch = Stopwatch.StartNew();

            return Next.Invoke(context).ContinueWith(t =>
            {
                Console.WriteLine("[{0} - {3}] - {1} in {2}ms with response {3} {4}",
                    context.Request.Method,
                    path,
                    stopWatch.ElapsedMilliseconds,
                    context.Response.StatusCode,
                    string.IsNullOrEmpty(context.Response.ReasonPhrase) ? "" : " - " + context.Response.ReasonPhrase);
                return t;
            });
        }
    }
}
