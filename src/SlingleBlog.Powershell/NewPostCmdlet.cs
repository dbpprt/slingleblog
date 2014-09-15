using System;
using System.Management.Automation;
using SlingleBlog.Powershell.Common;
using SlingleBlog.Powershell.Models;
using SlingleBlog.ViewModels;

namespace SlingleBlog.Powershell
{
    [Cmdlet(VerbsCommon.New, "Post")]
    public class NewPostCmdlet : Cmdlet
    {
        [Parameter(
            HelpMessage = "A blog instance",
            Mandatory = true
            )]
        public Blog Blog { get; set; }

        [Parameter(
            HelpMessage = "The posts title",
            Mandatory = true
            )]
        public string Title { get; set; }

        [Parameter(
            HelpMessage = "Some Tags",
            Mandatory = true
            )]
        public string[] Tags { get; set; }

        [Parameter(
            HelpMessage = "A slug. (must be url compatible!)",
            Mandatory = true
            )]
        public string Slug { get; set; }

        [Parameter(
            HelpMessage = "The posts markup",
            Mandatory = true
            )]
        public string Markup { get; set; }

        [Parameter(
            HelpMessage = "Publish the post after uploading"
            )]
        public SwitchParameter Publish { get; set; }

        protected override void ProcessRecord()
        {
            using (var api = new BlogApi(Blog))
            {
                WriteObject(
                    api.NewPost(new PostViewModel
                    {
                        Content = Markup,
                        Slug = Slug,
                        PubDate = DateTime.UtcNow,
                        Tags = Tags,
                        Title = Title
                    }));
            }
        }
    }
}
