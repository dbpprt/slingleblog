using System;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MobileDB.Contracts;
using OpenQA.Selenium.PhantomJS;
using SlingleBlog.Common.Configuration;
using SlingleBlog.Models;

namespace SlingleBlog.Common.PrerenderEngine
{
    class Prerenderer : IDisposable
    {
        private readonly IPrerendererSettings _configuration;
        private readonly IEntitySet<PrecompiledPage> _pages;
        private readonly IEntitySet<Post> _posts;

        private readonly PhantomJSDriverService _driverService;
        private readonly PhantomJSDriver _driver;

        private bool _disposed;

        public Prerenderer(
            IPrerendererSettings configuration,
            IEntitySet<PrecompiledPage> pages,
            IEntitySet<Post> posts
            )
        {
            _configuration = configuration;
            _pages = pages;
            _posts = posts;

            _driverService = PhantomJSDriverService.CreateDefaultService();
            _driver = new PhantomJSDriver(_driverService);
        }

        public async Task ProcessPendingPages(CancellationToken cancellationToken)
        {
            var prerenderBefore = DateTime.UtcNow.Subtract(new TimeSpan(_configuration.RecompileAfterHours, 0, 0));
            var pendingPages = (await _pages.AsQueryableAsync()).Where(_ => _.LastPrecompile < prerenderBefore).ToList();

            foreach (var pendingPage in pendingPages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var page = pendingPage; // weird closure thing -.-

                var pageSource = await Task.Factory.StartNew(
                    () => PrerenderPage(page.Url), cancellationToken);
            }

        }

        public string PrerenderPage(string url)
        {
            var manage = _driver.Manage();
            manage.Window.Maximize();

            _driver.Url = "http://localhost:8080/index.html";
            _driver.Navigate();
            Thread.Sleep(2000);
            
            _driver.GetScreenshot().SaveAsFile("C:\\slingle\\" + Guid.NewGuid() + ".png", ImageFormat.Png);

            var source = _driver.PageSource;
            return source;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _driver.Dispose();
                _driverService.Dispose();

                _disposed = true;
            }
        }
    }
}
