using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Helpers.Migrations;

public class Program
{
    public static IWebHost BuildWebHost(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
            .UseStartup(Assembly.GetCallingAssembly().FullName)
            .Build();
    }
}