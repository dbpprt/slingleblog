using System.Web.Http;
using MobileDB.Contracts;
using SlingleBlog.Common.Configuration;
using SlingleBlog.Models;

namespace SlingleBlog.Controllers
{
    public class PrerendererController : ApiController
    {
        private readonly IEntitySet<PrecompiledPage> _precompiledPages;
        private readonly IConfiguration _configuration;

        public PrerendererController(
            IEntitySet<PrecompiledPage> precompiledPages,
            IConfiguration configuration
            )
        {
            _precompiledPages = precompiledPages;
            _configuration = configuration;
        }
    }
}
