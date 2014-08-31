using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlingleBlog.Common.Configuration;

namespace SlingleBlog.Common.UrlRewrite
{
    public class RewriteRule : IRewriteRule
    {
        public string Name { get; set; }

        public string Template { get; set; }

        public string Constraint { get; set; }

        public string RewriteTo { get; set; }
    }
}
