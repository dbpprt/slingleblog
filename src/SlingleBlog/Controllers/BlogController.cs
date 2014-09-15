using System;
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
        private readonly IConfiguration _configuration;
        private readonly IEntitySet<Post> _posts;
        private readonly IDbContext _context;

        public BlogController(
            IConfiguration configuration,
            IEntitySet<Post> posts,
            IDbContext context
            )
        {
            _configuration = configuration;
            _posts = posts;
            _context = context;
        }

        [HttpGet]
        [Route("api/blog/posts/{page?}")]
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

        [HttpGet]
        [Route("api/blog/markup/{slug}")]
        public IHttpActionResult Markup(string slug)
        {
            var result = (from post in _posts.AsQueryable().ToList()
                          where post.Slug == slug
                          select new PostViewModel
                          {
                              Content = post.Content,
                              Slug = post.Slug,
                              PubDate = post.PubDate,
                              Tags = post.Tags,
                              Title = post.Title
                          }).FirstOrDefault();

            return result != null ? (IHttpActionResult)Ok(result) : NotFound();
        }

        [HttpPost]
        [Route("api/blog/post")]
        public IHttpActionResult NewPost(PostViewModel model)
        {
            var post = new Post
            {
                Content = model.Content,
                Slug = model.Slug,
                PubDate = DateTime.UtcNow,
                Title = model.Title,
                Tags = model.Tags,
                LastModified = DateTime.UtcNow
            };

            _posts.Add(post);
            _context.SaveChanges();

            return Ok(new PublishResult
            {
                Success = true,
                Url = "http://i-dont-know/"
            });
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
