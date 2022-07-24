using System.Collections.Generic;
using System.IO;
using System.Linq;
using DynamicData;
using Newtonsoft.Json;
using WorldManager.Services.Db;

namespace WorldManager.Services;

public class DbRepository
{
    [JsonProperty("Groups")] private List<string> _groups = new();

    [JsonProperty("Worlds")] private List<World> _worlds = new();

    [JsonIgnore] public SourceCache<World, string> Worlds { get; } = new(x => x.Id);

    [JsonIgnore] public SourceList<string> Groups { get; } = new();

    public static DbRepository Load()
    {
        var db = !File.Exists("./db.json")
            ? new DbRepository()
            : JsonConvert.DeserializeObject<DbRepository>(File.ReadAllText("./db.json"));

        db.Init();

        return db;
    }

    public void Init()
    {
        Worlds.AddOrUpdate(_worlds);

        Groups.Add("");
        Groups.AddRange(_groups);
    }

    public void Save()
    {
        _worlds = Worlds.Items.ToList();
        _groups = Groups.Items.ToList().FindAll(x => x != "");

        File.WriteAllText("./db.json", JsonConvert.SerializeObject(this));
    }
}