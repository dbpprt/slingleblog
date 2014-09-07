using System;
using Topshelf;

namespace SlingleBlog.Hosting.WindowsService
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            HostFactory.Run(hostConfigurator =>
            {
                hostConfigurator.Service<OwinHostService>(serviceConfigurator =>
                {
                    serviceConfigurator.ConstructUsing(() => new OwinHostService());
                    serviceConfigurator.WhenStarted(agentService => agentService.Start());
                    serviceConfigurator.WhenStopped(agentService => agentService.Stop());
                });

                hostConfigurator.RunAsNetworkService();

                hostConfigurator.BeforeInstall(() => NetAclChecker.AddAddress("http://+:8080/"));

                hostConfigurator.SetDisplayName("SinglePageBlog Service");
                hostConfigurator.SetDescription("Hosts a simple and cool single page blog.");
                hostConfigurator.SetServiceName("SinglePageBlog");
            });
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Console.WriteLine(unhandledExceptionEventArgs.ExceptionObject);
            Console.ReadLine();
        }
    }
}
