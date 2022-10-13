using Microsoft.AspNetCore;

namespace Migrations;

public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
        var startupAssemblyName = typeof(Program).Assembly.FullName;
        if (startupAssemblyName != null)
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup(startupAssemblyName)
                .Build();
        throw new ArgumentNullException(nameof(startupAssemblyName));
    }
}