using System;
using System.Linq;
using PhilipsHue.Cli.Handlers;

try
{
    if (!args.Any())
        throw new Exception("No arguments set. Type help for see list of arguments");

    var settingsHandler = new SettingsHandler();
    var settings = settingsHandler.GetSettings();

    if(settings == null)
    {
        settings = new Settings();
        Console.WriteLine("Enter IP-Address of Hue Bridge");
        settings.HueBrideIp = Console.ReadLine();

        Console.WriteLine("Enter User Id");
        settings.HueUserId = Console.ReadLine();

        settingsHandler.SaveSettings(settings);
    }

    LightHandler lightHandler = new LightHandler(settingsHandler.Settings);

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
        case "clear":
            settingsHandler.Clear();
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