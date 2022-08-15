using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Newtonsoft.Json;
using ReactiveUI;
using VRChat.API.Model;
using WorldManager.Services;

namespace WorldManager.ViewModels.Catalog;

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

    private DateTime? PublicationDate => World.PublicationDate == "none" ? null : DateTime.Parse(World.PublicationDate);

    private DateTime UpdateDate => World.UpdatedAt;

    public string PublicationDateFormat
    {
        get
        {
            var result = PublicationDate?.ToShortDateString();
            return result ?? "Неизвестно";
        }
    }

    public string UpdateDateFormat => UpdateDate.ToShortDateString();

    public async Task LoadThumbnail()
    {
        await using var stream = await WorksWithUrlImage.LoadImageFromUrl(
            World.ThumbnailImageUrl, World.Id + ".bmp",
            new Task<Stream>(() =>
            {
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                return assets?.Open(new Uri("avares://WorldManager/Assets/default_world_thumb.png")) ??
                       new MemoryStream();
            })
        );
        Thumbnail = await Task.Run(() => Bitmap.DecodeToWidth(stream, 256));
    }
}