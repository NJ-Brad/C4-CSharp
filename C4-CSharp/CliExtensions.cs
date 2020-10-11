using C4_Builder_Net;
using C4_CSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;

    public static class CliCommandCollectionExtensions
{
    /// <summary>
    /// Adds the CLI commands to the DI container. These are resolved when the commands are registered with the
    /// <c>CommandLineBuilder</c>.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <returns>The service collection, for chaining.</returns>
    /// <remarks>
    /// We are using convention to register the commands; essentially everything in the same namespace as the
    /// <see cref="DeployCommand"/> and that implements <c>Command</c> will be registered. If any commands are
    /// added in other namespaces, this method will need to be modified/extended to deal with that.
    /// </remarks>
    public static IServiceCollection AddCliCommands(this IServiceCollection services)
    {
        Type grabCommandType = typeof(BuildCommand);
        Type commandType = typeof(Command);

        IEnumerable<Type> commands = grabCommandType
            .Assembly
            .GetExportedTypes()
            .Where(x => x.Namespace == grabCommandType.Namespace && commandType.IsAssignableFrom(x));

        foreach (Type command in commands)
        {
            services.AddSingleton(commandType, command);
        }

        //services.AddSingleton(sp =>
        //{
        //    return
        //       sp.GetRequiredService<IConfiguration>().GetSection("Greet").Get<GreetOptions>()
        //       ?? throw new ArgumentException("Greeting configuration cannot be missing.");
        //});

        services.AddSingleton(sp =>
        {
            return
               sp.GetRequiredService<IConfiguration>().GetSection("BuildCommand").Get<BuildCommandOptions>()
               ?? throw new ArgumentException("Build configuration cannot be missing.");
        });

        //services.AddSingleton(sp =>
        //{
        //    return
        //       sp.GetRequiredService<IConfiguration>().GetSection("General").Get<GeneralOptions>()
        //       ?? throw new ArgumentException("General configuration cannot be missing.");
        //});


        return services;
    }
}
