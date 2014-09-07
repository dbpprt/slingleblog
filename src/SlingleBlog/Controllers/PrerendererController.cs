using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FileBiggy.Contracts;
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
