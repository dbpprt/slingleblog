using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SlingleBlog.Powershell.Models;

namespace SlingleBlog.Powershell
{
    [Cmdlet(VerbsCommon.Get, "Jobs")]
    public class GetJobsCmdlet : Cmdlet
    {

        [Parameter(
            HelpMessage = "A blog instance",
            Mandatory = true
            )]
        public Blog Blog { get; set; }

        protected override void ProcessRecord()
        {
            WriteVerbose("Connecting to blog " + Blog.Endpoint);
        }
    }
}
