
using PhilipsHue.Cli.Handlers;
LightHandler lightHandler = new();
var light = "";
var desiredState = "";
var query = args.AsQueryable();

if (args.Length > 1 && args[1] is "on" or "off")
{
    desiredState = args[1];
}
if (args[0] is "lights")
{
    await lightHandler.GetLights(true);
}
if (args[0] is "help")
{
    Console.WriteLine("Choose an option from the following list:");
    Console.WriteLine("\t- lights");
    Console.WriteLine("\t- <light name/id> on");
    Console.WriteLine("\t- <light name/id> off");
}
if (args[0] is not "lights" && args[0] is not "help")
{
    light = args[0];
    var lights = await lightHandler.GetLights(false);
    await lightHandler.SetLightState(light, desiredState, lights);
}
// Debugging
//Console.WriteLine($"{light} - {desiredState}");

