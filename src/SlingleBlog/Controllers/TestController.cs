using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using SlingleBlog.Common.Configuration;

namespace SlingleBlog.Controllers
{
    public class TestController : ApiController
    {
        private readonly IConfiguration _configuration;

        public TestController(
            IConfiguration configuration
            )
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("api/test")]
        public IHttpActionResult Test()
        {
            return Ok("it works");
        }
    }
}
