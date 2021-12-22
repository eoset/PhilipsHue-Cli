using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using PhilipsHue.Cli.Handlers;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

try
{

    if (!args.Any())
        throw new Exception("No arguments set. Type help for see list of arguments");

    LightHandler lightHandler = new LightHandler(configuration);

    switch (args[0])
    {
        case "lights":
            await lightHandler.GetLightsOutPut();
            break;
        case "help":
            Console.WriteLine("Choose an option from the following list:");
            Console.WriteLine("\t- lights");
            Console.WriteLine("\t- <light name/id> on");
            Console.WriteLine("\t- <light name/id> off");
            break;
        default:
            if (args.Length == 1)
                throw new Exception("Missing argument for desired State");

            var response = await lightHandler.SetLightState(args[0], args[1]);
            Console.WriteLine(response);

            break;
    }

}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}