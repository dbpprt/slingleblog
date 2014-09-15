using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SlingleBlog.Powershell.Common;
using SlingleBlog.Powershell.Models;

namespace SlingleBlog.Powershell
{
    [Cmdlet(VerbsCommon.Get, "Post")]
    public class GetPost : Cmdlet
    {

        [Parameter(
            HelpMessage = "A blog instance",
            Mandatory = true
            )]
        public Blog Blog { get; set; }

        [Parameter(
            HelpMessage = "The posts slug",
            ValueFromPipeline = true
            )]
        public string Slug { get; set; }

        protected override void ProcessRecord()
        {
            WriteVerbose("Connecting to blog " + Blog.Endpoint);

            if (string.IsNullOrEmpty(Slug))
            {
                using (var api = new BlogApi(Blog))
                {
                    WriteObject(api.GetPosts());
                }
            }
            else
            {
                using (var api = new BlogApi(Blog))
                {
                    WriteObject(api.GetPost(Slug));
                }
            }
        }
    }
}
