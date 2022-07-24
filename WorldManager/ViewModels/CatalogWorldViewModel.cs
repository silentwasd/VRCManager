using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Newtonsoft.Json;
using ReactiveUI;
using VRChat.API.Model;
using WorldManager.Services;
using File = System.IO.File;

namespace WorldManager.ViewModels;

public class CatalogWorldViewModel : ViewModelBase
{
    private string _json = "";
    private Bitmap? _thumbnail;

    public CatalogWorldViewModel()
    {
        var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
        var worldStream = assets?.Open(new Uri("avares://WorldManager/Assets/default_world.json"));

        if (worldStream == null)
        {
            World = new LimitedWorld();
            return;
        }

        var worldData = new StreamReader(worldStream).ReadToEnd();
        World = JsonConvert.DeserializeObject<LimitedWorld>(worldData);
    }

    public CatalogWorldViewModel(LimitedWorld world)
    {
        World = world;
        _json = world.ToJson();
    }

    public LimitedWorld World { get; }

    public Bitmap? Thumbnail
    {
        get => _thumbnail;
        set => this.RaiseAndSetIfChanged(ref _thumbnail, value);
    }

    public string Json
    {
        get => _json;
        set => this.RaiseAndSetIfChanged(ref _json, value);
    }

    public DateTime? PublicationDate => World.PublicationDate == "none" ? null : DateTime.Parse(World.PublicationDate);

    public DateTime UpdateDate => World.UpdatedAt;

    public string PublicationDateFormat
    {
        get
        {
            var result = PublicationDate?.ToShortDateString();
            return result ?? "Неизвестно";
        }
    }

    public string UpdateDateFormat => UpdateDate.ToShortDateString();

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