using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhilipsHue.Cli.Models;

namespace PhilipsHue.Cli.Handlers
{
    public class LightHandler
    {
        private readonly HttpClient _httpClient;
        private List<Light> Lights = new List<Light>();

        public LightHandler(Settings settings)
        {
            //var bridgeIp = configuration.GetSection("HueBrideIP").Value;
            //var userId = configuration.GetSection("HueUserId").Value;
            //

            _httpClient = CreateClient(settings.HueBrideIp, settings.HueUserId);
        }

        private HttpClient CreateClient(string bridgeIp, string userId)
        {
            var httpClientHandler = new HttpClientHandler();

            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
            var client = new HttpClient(httpClientHandler);
            client.BaseAddress = new Uri("https://" + bridgeIp + $"/api/{userId}/");
            return client;
        }

        public async Task GetLights()
        {
            if (Lights.Any())
                return;

            var response = await _httpClient.GetStringAsync("lights");

            var token = JToken.Parse(response);
            if (token.Type != JTokenType.Object) return;
            var jsonResult = (JObject) token;

            foreach (var prop in jsonResult.Properties())
            {
                Light? newLight = JsonConvert.DeserializeObject<Light>(prop.Value.ToString());
                if (newLight != null)
                {
                    newLight.Id = prop.Name;
                    Lights.Add(newLight);
                }
            }
        }

        public async Task GetLightsOutPut()
        {
            await GetLights();

            foreach (var light in Lights)
                Console.WriteLine($"Name: {light.Name}, State: {light.State.On}, {light.Id}");
        }

        public async Task<string> SetLightState(string light, string state)
        {
            await GetLights();

            Light lightToUse = Lights.First(l => l.Name.ToLower() == light.ToLower() || l.Id == light);

            var hueState = new HueState();

            switch (state)
            {
                case "on":
                    hueState.on = true;
                    break;
                case "off":
                    hueState.on = false;
                    break;
                default:
                    throw new Exception("state argument must be either \"on\" or \"off\"");
            }

            var objAsJson = JsonConvert.SerializeObject(hueState);
            StringContent content = new StringContent(objAsJson, Encoding.UTF8, "application/json");

            var response = _httpClient.PutAsync($"lights/{lightToUse.Id}/state", content).Result;

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error response code " + response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        private class HueState
        {
            public bool @on { get; set; }
        }
    }
}