using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SlingleBlog.Powershell.Models;

namespace SlingleBlog.Powershell.Common
{
    public class BlogApi
    {
        private readonly Blog _blog;
        private readonly HttpClient _client;

        public BlogApi(Blog blog)
        {
            _blog = blog;
            var client = new HttpClient
            {
                BaseAddress = new Uri(blog.Endpoint)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                
                // New code:
                //HttpResponseMessage response = await client.GetAsync("api/products/1");
                //if (response.IsSuccessStatusCode)
                //{
                //    Product product = await response.Content.ReadAsAsync > Product > ()
                //    ;
                //    Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
                //}
            }
        }
    }
}
