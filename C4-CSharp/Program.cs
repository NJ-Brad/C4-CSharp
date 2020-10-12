using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using System.CommandLine;
using System.CommandLine.Invocation;
using System.Reflection;
using System.IO;
using C4_CSharp.AssemblyDiagram;

namespace C4_CSharp
{
    // Handling of the command line and options
    // https://endjin.com/blog/2020/09/simple-pattern-for-using-system-commandline-with-dependency-injection.html
    // https://github.com/carmeleve/SystemCommandLine.Demo

    // Overall process
    // https://adrianvlupu.github.io/C4-Builder/#/
    // https://github.com/adrianvlupu/C4-Builder

    internal static class Program
    {
        /// <summary>
        /// The entry point for the program.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>When complete, an integer representing success (0) or failure (non-0).</returns>
        public static async Task<int> Main(string[] args)
        {
            ServiceProvider serviceProvider = BuildServiceProvider();
            Parser parser = BuildParser(serviceProvider);

            return await parser.InvokeAsync(args).ConfigureAwait(false);
        }

        private static Parser BuildParser(ServiceProvider serviceProvider)
        {
            var commandLineBuilder = new CommandLineBuilder();

            foreach (Command command in serviceProvider.GetServices<Command>())
            {
                commandLineBuilder.AddCommand(command);
            }

            return commandLineBuilder.UseDefaults().Build();
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            IConfigurationRoot config = new ConfigurationBuilder()

                    //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.json")
                    .Build();

            services.AddSingleton<IConfiguration>(config);
            services.AddCliCommands();

            return services.BuildServiceProvider();
        }
    }
}


//  -V, --version   output the version number
//  new create a new project from template
//  config          change configuration for the current directory
//    list            display the current configuration
//    reset           clear all configuration
//  site            serve the generated site
//  -w, --watch     watch for changes and rebuild
//  docs            a brief explanation for the available configuration options
//  -p, --port <n>  port used for serving the generated site
//  -h, --help      output usage information
