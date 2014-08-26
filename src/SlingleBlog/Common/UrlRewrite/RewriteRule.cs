using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlingleBlog.Common.UrlRewrite
{
    public class RewriteRule
    {
        public string Name { get; set; }

        public string Template { get; set; }

        public string Constraint { get; set; }

        public string RewriteTo { get; set; }
    }
}
