using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Newtonsoft.Json;
using ReactiveUI;
using VRChat.API.Model;
using WorldManager.Services;
using File = System.IO.File;
using World = WorldManager.Services.Db.World;

namespace WorldManager.ViewModels;

public class SavedWorldViewModel : ViewModelBase
{
    private Bitmap? _thumbnail;

    private IBrush _worldBackground = Brushes.Transparent;

    public SavedWorldViewModel()
    {
        var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
        var worldStream = assets?.Open(new Uri("avares://WorldManager/Assets/default_world.json"));

        if (worldStream == null)
        {
            World = World.FromLimited(new LimitedWorld());
            return;
        }

        var worldData = new StreamReader(worldStream).ReadToEnd();
        World = World.FromLimited(JsonConvert.DeserializeObject<LimitedWorld>(worldData));
    }

    public SavedWorldViewModel(World world, SavedWorldsViewModel parent)
    {
        World = world;
        Parent = parent;

        Parent.WhenAnyValue(x => x.SelectedWorld)
            .Subscribe(selectedWorld =>
            {
                if (selectedWorld == null || selectedWorld.World.Id != World.Id)
                {
                    WorldBackground = Brush.Parse("#d9ebfc");
                    return;
                }

                WorldBackground = Brush.Parse("#3675b3");
            });
    }

    public IBrush WorldBackground
    {
        get => _worldBackground;
        set => this.RaiseAndSetIfChanged(ref _worldBackground, value);
    }

    private SavedWorldsViewModel Parent { get; }

    public World World { get; }

    public Bitmap? Thumbnail
    {
        get => _thumbnail;
        set => this.RaiseAndSetIfChanged(ref _thumbnail, value);
    }

    private async Task<Stream> LoadThumbnailBitmap()
    {
        var id = World.Id;
        if (File.Exists("images/" + id + ".bmp"))
        {
            return new MemoryStream(await File.ReadAllBytesAsync("images/" + id + ".bmp"));
        }
        else
        {
            var response = AppCompose.HttpClient.GetAsync(World.ThumbnailImageUrl);

            if (response.Result.StatusCode != HttpStatusCode.OK)
            {
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                return assets?.Open(new Uri("avares://WorldManager/Assets/default_world_thumb.png")) ??
                       new MemoryStream();
            }

            var data = await response.Result.Content.ReadAsByteArrayAsync();

            if (!Directory.Exists("images"))
                Directory.CreateDirectory("images");

            await File.WriteAllBytesAsync("images/" + id + ".bmp", data);

            return new MemoryStream(data);
        }
    }

    public async Task LoadThumbnail()
    {
        await using var stream = await LoadThumbnailBitmap();
        Thumbnail = await Task.Run(() => Bitmap.DecodeToWidth(stream, 256));
    }
}