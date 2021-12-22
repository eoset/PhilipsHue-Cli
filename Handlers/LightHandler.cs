using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PhilipsHue.Cli.Handlers;

public class LightHandler
{
    public async Task<List<Light>> GetLights(bool output)
    {
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
        var httpClient = new HttpClient(httpClientHandler);

        var response =
            await httpClient.GetStringAsync(
                new Uri("https://192.168.50.91/api/<insert token>/lights"));
                

        List<Light> results = new();

        var token = JToken.Parse(response);
        if (token.Type != JTokenType.Object) return results;
        var jsonResult = (JObject) token;

        foreach (var prop in jsonResult.Properties())
        {
            Light? newLight = JsonConvert.DeserializeObject<Light>(prop.Value.ToString());
            if (newLight != null)
            {
                newLight.Id = prop.Name;
                results.Add(newLight);
                if(output)
                    Console.WriteLine($"Name: {newLight.Name}, State: {newLight.State.On}, {newLight.Id}");
            }
        }
        return results;
    }

    public async Task<string> SetLightState(string light, string state, List<Light> lights)
    {
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
        var lightToUse = lights.FirstOrDefault(l => l.Name.ToLower() == light.ToLower() || l.Id == light).Id;
        var httpClient = new HttpClient(httpClientHandler);
        var myObject = new State()
        {
            On = true
        };
        switch (state)
        {
            case "on":
                myObject.On = true;
                break;
            case "off":
                myObject.On = false;
                break;
        }
        
        var objAsJson = JsonConvert.SerializeObject(myObject);
        var content = new StringContent(objAsJson, Encoding.UTF8, "application/json");
        var response =
            await httpClient.PutAsync(
                new Uri($"https://192.168.50.91/api/<insert token>/lights{lightToUse}/state"), content);
                
        if (!response.IsSuccessStatusCode)
        {
            return "failed";
        }

        return "success";
    }
}