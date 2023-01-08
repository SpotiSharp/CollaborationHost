using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotiSharpBackend;

namespace CollaborationHost.Models;

public class ConfigurationManager
{
    private static ConfigurationManager _configurationManager;
    
    public static ConfigurationManager Instance => _configurationManager ??= new ConfigurationManager();
    
    private string _configPath = @"Configuration\config.json";
    
    private Dictionary<string, string>? _config = new Dictionary<string, string>();
    
    private ConfigurationManager()
    {
        _config = JObject.Parse(File.ReadAllText(_configPath)).ToObject<Dictionary<string, string>>();
        if (_config != null)
        {
            if (_config.TryGetValue("clientId", out string clientId)) StorageHandler.ClientId = clientId;
            if (_config.TryGetValue("clientSecret", out string clientSecret)) StorageHandler.ClientSecret = clientSecret;
        }
        StorageHandler.OnDataChange += UpdateConfig;
    }

    private void UpdateConfig(string key, string value)
    {
        if (_config == null) return;
        File.WriteAllText(_configPath, JsonConvert.SerializeObject(_config));
    }
}