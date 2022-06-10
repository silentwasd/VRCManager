namespace WorldManager.Services;

public class AppCompose
{
    private static Config? _config;

    public static Config Config
    {
        get
        {
            var config = _config ?? Config.Load();
            _config = config;
            return config;
        }
    }
}