using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ReactiveUI;
using VRChat.API.Model;
using WorldManager.Services;
using File = System.IO.File;

namespace WorldManager.ViewModels;

public class CatalogWorldViewModel : ViewModelBase
{
    private LimitedWorld _world;

    private Bitmap? _thumbnail;

    private string _json;
    
    public CatalogWorldViewModel()
    {
        _world = new LimitedWorld();
    }
    
    public CatalogWorldViewModel(LimitedWorld world)
    {
        _world = world;
        _json = world.ToJson();
    }

    public LimitedWorld World => _world;

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
            if (PublicationDate == null)
                return "Неизвестно";
            return PublicationDate?.ToShortDateString();
        }
    }

    public string UpdateDateFormat => UpdateDate.ToShortDateString();

    private async Task<Stream> LoadThumbnailBitmap()
    {
        var id = World.Id;
        if (File.Exists("images/" + id + ".bmp"))
        {
            return new MemoryStream(File.ReadAllBytes("images/" + id + ".bmp"));
        }
        else
        {
            var data = await AppCompose.HttpClient.GetByteArrayAsync(World.ThumbnailImageUrl);
            
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