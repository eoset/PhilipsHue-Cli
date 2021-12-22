using System.IO;
using Newtonsoft.Json;

namespace PhilipsHue.Cli.Handlers
{
    public class SettingsHandler
    {
        private readonly string _filePath;

        public Settings? Settings { get; private set; }

        public SettingsHandler()
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");
        }

        public void SaveSettings(Settings settings)
        {
            var contents = JsonConvert.SerializeObject(settings);
            File.WriteAllText(_filePath, contents);

            Settings = settings;
        }

        public Settings? GetSettings()
        {
            if (!File.Exists(_filePath))
                return null;

            var content = File.ReadAllText(_filePath);
            var settings = JsonConvert.DeserializeObject<Settings>(content);

            Settings = settings;

            return Settings;
        }

        public void Clear()
        {
            if (!File.Exists(_filePath))
                File.Delete(_filePath);
        }

    }

    public class Settings
    {
        public string HueBrideIp { get; set; }
        public string HueUserId { get; set; }
    }
}