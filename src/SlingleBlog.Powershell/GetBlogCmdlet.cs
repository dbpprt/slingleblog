using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SlingleBlog.Powershell.Models;

namespace SlingleBlog.Powershell
{
    [Cmdlet(VerbsCommon.Get, "Blog")]
    public class GetBlogCmdlet : Cmdlet
    {
        [Parameter(
            HelpMessage = "The api key for the remote blog instance",
            Mandatory = true
            )]
        public string ApiKey { get; set; }

        [Parameter(
            HelpMessage = "The uri for the blog",
            Mandatory = true
            )]
        public string Endpoint { get; set; }

        protected override void BeginProcessing()
        {
            WriteObject(new Blog
            {
                ApiKey = ApiKey,
                Endpoint = Endpoint
            });
        }
    }
}
