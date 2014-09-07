using System;
using System.Diagnostics;

namespace SlingleBlog.Hosting.WindowsService
{
    public static class NetAclChecker
    {
        public static void AddAddress(string address)
        {
            AddAddress(address, Environment.UserDomainName, Environment.UserName);
        }

        private static void AddAddress(string address, string domain, string user)
        {
            var args = string.Format(@"http add urlacl url={0} user=""{1}\{2}""", address, domain, user);
 
            var psi = new ProcessStartInfo("netsh", args)
            {
                Verb = "runas",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            };

            Process.Start(psi).WaitForExit();
        }
    }
}
