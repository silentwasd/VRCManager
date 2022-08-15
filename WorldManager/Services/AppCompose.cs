using System.Net.Http;
using System.Net.Http.Headers;
using WorldManager.ViewModels;

namespace WorldManager.Services;

public static class AppCompose
{
    private static Config? _config;

    private static DbRepository? _dbRepository;

    public static MainWindowViewModel? MainWindow { get; set; }

    public static Config Config
    {
        get
        {
            var config = _config ?? Config.Load();
            _config = config;
            return config;
        }
    }

    public static DbRepository DbRepository
    {
        get
        {
            var db = _dbRepository ?? DbRepository.Load();
            _dbRepository = db;
            return db;
        }
    }

    public static HttpClient HttpClient
    {
        get
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("VRCManager", "1.0.0"));
            return client;
        }
    }
}