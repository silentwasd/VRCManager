using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using WorldManager.Services.Db;

namespace WorldManager.Services;

public class DbRepository
{
    public static DbRepository Load()
    {
        return !File.Exists("./db.json")
            ? new DbRepository()
            : JsonConvert.DeserializeObject<DbRepository>(File.ReadAllText("./db.json"));
    }

    public void Save()
    {
        File.WriteAllText("./db.json", JsonConvert.SerializeObject(this));
    }

    public List<World> Worlds { get; } = new();
}