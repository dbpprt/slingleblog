using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using SlingleBlog.Powershell.Models;
using SlingleBlog.ViewModels;

namespace SlingleBlog.Powershell.Common
{
    public class BlogApi : IDisposable
    {
        private readonly Blog _blog;
        private readonly HttpClient _client;

        public BlogApi(Blog blog)
        {
            _blog = blog;
            _client = new HttpClient
            {
                BaseAddress = new Uri(blog.Endpoint)
            };

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // New code:
            //HttpResponseMessage response = await client.GetAsync("api/products/1");
            //if (response.IsSuccessStatusCode)
            //{
            //    Product product = await response.Content.ReadAsAsync > Product > ()
            //    ;
            //    Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
            //}
        }

        private void ThrowIfUnsuccessfully(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public IEnumerable<JobViewModel> GetJobs()
        {
            var response = _client.GetAsync("api/sys/jobs").Result;
            ThrowIfUnsuccessfully(response);
            var jobs =
                JsonConvert.DeserializeObject<IEnumerable<JobViewModel>>(response.Content.ReadAsStringAsync().Result);

            return jobs;
        }

        public PostViewModel GetPost(string slug)
        {
            var response = _client.GetAsync("api/blog/post/" + slug).Result;
            ThrowIfUnsuccessfully(response);
            var post =
                JsonConvert.DeserializeObject<PostViewModel>(response.Content.ReadAsStringAsync().Result);

            return post;
        }

        public IEnumerable<PostViewModel> GetPosts(int page)
        {
            var response = _client.GetAsync("api/blog/posts/" + page).Result;
            ThrowIfUnsuccessfully(response);
            var posts =
                JsonConvert.DeserializeObject<IEnumerable<PostViewModel>>(response.Content.ReadAsStringAsync().Result);

            return posts;
        }

        public PublishResult NewPost(PostViewModel post)
        {
            var response = _client.PostAsync("api/blog/post", Json(post)).Result;
            ThrowIfUnsuccessfully(response);
            var result =
                JsonConvert.DeserializeObject<PublishResult>(response.Content.ReadAsStringAsync().Result);

            return result;
        }

        public HttpContent Json(object obj)
        {
            return new StringContent(
                JsonConvert.SerializeObject(obj),
                Encoding.UTF8,
                "application/json");
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
