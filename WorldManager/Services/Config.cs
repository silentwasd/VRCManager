using System.IO;
using Newtonsoft.Json;

namespace WorldManager.Services;

public class Config
{
    public string? AuthCookie { get; set; }

    public static Config Load()
    {
        return !File.Exists("./config.json") ? new Config() : 
            JsonConvert.DeserializeObject<Config>(File.ReadAllText("./config.json"));
    }

    public void Save()
    {
        File.WriteAllText("./config.json", JsonConvert.SerializeObject(this));
    }
}