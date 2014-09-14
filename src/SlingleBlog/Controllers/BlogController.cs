using System.Linq;
using System.Web.Http;
using MobileDB.Contracts;
using SlingleBlog.Common.Configuration;
using SlingleBlog.Models;
using SlingleBlog.ViewModels;

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
            var result = (from post in _posts.AsQueryable().ToList()
                          let content = post.RenderPreviewMarkup()
                          select new PostViewModel
                          {
                              Content = content,
                              Slug = post.Slug,
                              PubDate = post.PubDate,
                              Tags = post.Tags,
                              Title = post.Title
                          }).ToList();

            return Json(result);
        }

        [HttpGet]
        [Route("api/blog/post/{slug}")]
        public IHttpActionResult Post(string slug)
        {
            var result = (from post in _posts.AsQueryable().ToList()
                where post.Slug == slug
                let content = post.RenderMarkup()
                select new PostViewModel
                {
                    Content = content,
                    Slug = post.Slug,
                    PubDate = post.PubDate,
                    Tags = post.Tags,
                    Title = post.Title
                }).FirstOrDefault();

            return result != null ? (IHttpActionResult) Ok(result) : NotFound();
        }

        [HttpPost]
        [Route("api/blog/post")]
        public IHttpActionResult NewPost(PostViewModel model)
        {

            return Ok();
        }

        [HttpGet]
        [Route("api/blog/tags")]
        public IHttpActionResult Tags()
        {
            var posts = _posts.AsQueryable().ToList();

            var result = (from tag in posts
                .SelectMany(post => post.Tags)
                .Distinct()
                let count = posts.Count(article => article.Tags.Contains(tag))
                select new TagViewModel
                {
                    Name = tag,
                    Posts = count
                }).ToList();
            
            return Ok(result);
        }

        [HttpGet]
        [Route("api/blog/tag/{tag}")]
        public IHttpActionResult PostsByTag(string tag)
        {
            var result = (from post in _posts.AsQueryable().ToList()
                          where post.Tags.Contains(tag)
                          let content = post.RenderPreviewMarkup()
                          select new PostViewModel
                          {
                              Content = content,
                              Slug = post.Slug,
                              PubDate = post.PubDate,
                              Tags = post.Tags,
                              Title = post.Title
                          }).FirstOrDefault();

            return result != null ? (IHttpActionResult)Ok(result) : NotFound();
        }
    }
}
