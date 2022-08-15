using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicData;
using DynamicData.Alias;
using Newtonsoft.Json;
using ReactiveUI;
using VRChat.API.Api;
using VRChat.API.Client;
using WorldManager.Services;
using WorldManager.Services.Db;

namespace WorldManager.ViewModels.WorldDetails;

public class WorldViewModel : ViewModelBase
{
    private string _favorites;

    private bool _hasPreviewYoutube;

    private Bitmap _image;

    private bool _previewYoutubeCopied;

    private string _publicationDateFormat;

    private bool _saved;

    private World? _savedWorld;

    private TagViewModel[] _tags;

    private string _updateDateFormat;

    private string _visits;
    private Models.VrcApi.World _world;

    private readonly WorldsApi _worldsApi;

    public WorldViewModel()
    {
    }

    public WorldViewModel(Configuration apiConfig, string worldId)
    {
        _worldsApi = new WorldsApi(apiConfig);
        Task.Run(() => LoadWorld(worldId));
    }

    public Models.VrcApi.World World
    {
        get => _world;
        set => this.RaiseAndSetIfChanged(ref _world, value);
    }

    public World? SavedWorld
    {
        get => _savedWorld;
        set => this.RaiseAndSetIfChanged(ref _savedWorld, value);
    }

    public Bitmap Image
    {
        get => _image;
        set => this.RaiseAndSetIfChanged(ref _image, value);
    }

    public string PublicationDateFormat
    {
        get => _publicationDateFormat;
        set => this.RaiseAndSetIfChanged(ref _publicationDateFormat, value);
    }

    public string UpdateDateFormat
    {
        get => _updateDateFormat;
        set => this.RaiseAndSetIfChanged(ref _updateDateFormat, value);
    }

    public TagViewModel[] Tags
    {
        get => _tags;
        set => this.RaiseAndSetIfChanged(ref _tags, value);
    }

    public bool HasPreviewYoutube
    {
        get => _hasPreviewYoutube;
        set => this.RaiseAndSetIfChanged(ref _hasPreviewYoutube, value);
    }

    public bool PreviewYoutubeCopied
    {
        get => _previewYoutubeCopied;
        set => this.RaiseAndSetIfChanged(ref _previewYoutubeCopied, value);
    }

    public bool Saved
    {
        get => _saved;
        set => this.RaiseAndSetIfChanged(ref _saved, value);
    }

    public string Visits
    {
        get => _visits;
        set => this.RaiseAndSetIfChanged(ref _visits, value);
    }

    public string Favorites
    {
        get => _favorites;
        set => this.RaiseAndSetIfChanged(ref _favorites, value);
    }

    public async void OpenPreviewYoutube()
    {
        if (!HasPreviewYoutube)
            return;

        if (Application.Current is {Clipboard: { }})
        {
            await Application.Current.Clipboard.SetTextAsync("https://youtube.com/watch?v=" + World.PreviewYoutubeId);

            PreviewYoutubeCopied = true;

            await Task.Delay(3000);

            PreviewYoutubeCopied = false;
        }
    }

    public void Save()
    {
        var world = Services.Db.World.FromFull(World);

        AppCompose.DbRepository.Worlds.AddOrUpdate(world);

        AppCompose.DbRepository.Save();
    }

    public void Remove()
    {
        AppCompose.DbRepository.Worlds.Remove(
            AppCompose.DbRepository.Worlds.Items.First(world => world.Id == World.Id));
        AppCompose.DbRepository.Save();
    }

    public void SaveNote()
    {
        AppCompose.DbRepository.Save();
    }

    private async void LoadWorld(string worldId)
    {
        var response = await _worldsApi.GetWorldWithHttpInfoAsync(worldId);

        World = JsonConvert.DeserializeObject<Models.VrcApi.World>(response.RawContent);

        SetFields();
        SetImage();
        SetDynamicSaveField();
    }

    private void SetFields()
    {
        PublicationDateFormat = World.PublicationDate == "none"
            ? "Неизвестно"
            : DateTime.Parse(World.PublicationDate).ToShortDateString();

        UpdateDateFormat = World.UpdatedAt == "none"
            ? "Неизвестно"
            : DateTime.Parse(World.UpdatedAt).ToShortDateString();

        Visits = World.Visits.ToString("#,#", new NumberFormatInfo {NumberGroupSeparator = " "});

        Favorites = World.Favorites.ToString("#,#", new NumberFormatInfo {NumberGroupSeparator = " "});

        Tags = World.Tags.Select(tag => new TagViewModel(tag)).OrderBy(tag => tag.Name).ToArray();

        HasPreviewYoutube = !string.IsNullOrEmpty(World.PreviewYoutubeId);
    }

    private async void SetImage()
    {
        await using var imageStream = await WorksWithUrlImage.LoadImageFromUrl(
            World.ImageUrl, World.Id + "_full.bmp",
            new Task<Stream>(() =>
            {
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                return assets?.Open(new Uri("avares://WorldManager/Assets/default_world_thumb.png")) ??
                       new MemoryStream();
            })
        );

        Image = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
    }

    private void SetDynamicSaveField()
    {
        AppCompose.DbRepository.Worlds.Connect()
            .Where(x => x.Id == World.Id)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var worlds)
            .DisposeMany()
            .Subscribe();

        worlds.WhenAnyValue(x => x.Count)
            .Subscribe(x =>
            {
                Saved = x > 0;
                SavedWorld = x > 0 ? worlds[0] : null;
            });
    }
}