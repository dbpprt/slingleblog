using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FileBiggy.Contracts;
using SlingleBlog.Common.Configuration;
using SlingleBlog.Common.Utilities;
using SlingleBlog.Models;

namespace SlingleBlog.Controllers
{
    public class BlogController : ApiController
    {
        private readonly IEntitySet<Post> _posts;

        public BlogController(
            IConfiguration configuration,
            IEntitySet<Post> posts 
            )
        {
            _posts = posts;
        }

        [HttpGet]
        [Route("api/blog/posts/{page}")]
        public IHttpActionResult Posts(int? page)
        {
            var articles = _posts.ToList()
                .Select(article => article.RenderPreviewMarkup());

            return Json(articles);
        }

        [HttpGet]
        [Route("api/blog/post/{slug}")]
        public IHttpActionResult Post(string slug)
        {
            var article = _posts.FirstOrDefault(_ => _.Slug == slug.Trim());

            if (article != null)
            {
                return Json(article.RenderMarkup());
            }

            return NotFound();
        }
 
        [HttpGet]
        [Route("api/blog/tags")]
        public IHttpActionResult Tags()
        {
            var articles = _posts.ToList();
            var tags = articles
                .SelectMany(article => article.Tags)
                .Distinct();
            var result = tags.Select(tag => new Tag
            {
                Name = tag,
                Count = articles.Count(article => article.Tags.Contains(tag))
            });

            return Json(result);
        }

        [HttpGet]
        [Route("api/blog/tag/{tag}")]
        public IHttpActionResult PostsByTag(string tag)
        {
            var articles = _posts.ToList()
                .Where(article => article.Tags.Contains(tag))
                .Select(article => article.RenderPreviewMarkup())
                .ToList();

            return Json(articles);
        }
    }
}
