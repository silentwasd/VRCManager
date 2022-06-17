using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using VRChat.API.Api;
using VRChat.API.Client;

namespace WorldManager.ViewModels;

public class CatalogViewModel : ViewModelBase
{
    private Configuration _apiConfig;

    private CatalogWorldViewModel _selectedWorld;

    private string _search;

    private KeyValuePair<string, string> _sort;

    private bool _quest;

    private bool _featured;

    public CatalogViewModel()
    {
        _apiConfig = new Configuration();
    }
    
    public CatalogViewModel(Configuration apiConfig)
    {
        _apiConfig = apiConfig;

        Sort = new KeyValuePair<string, string>("popularity", "популярность");
        Quest = false;

        this.WhenAnyValue(x => x.Search)
            .Throttle(TimeSpan.FromSeconds(1))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x =>
            {
                LoadFoundWorlds();
            });
        
        this.WhenAnyValue(x => x.Featured)
            .Throttle(TimeSpan.FromSeconds(1))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x =>
            {
                LoadFoundWorlds();
            });
        
        this.WhenAnyValue(x => x.Sort)
            .Throttle(TimeSpan.FromSeconds(1))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x =>
            {
                if (x.Key != String.Empty)
                    LoadFoundWorlds();
            });
        
        this.WhenAnyValue(x => x.Quest)
            .Throttle(TimeSpan.FromSeconds(1))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x =>
            {
                LoadFoundWorlds();
            });
    }

    private void LoadActiveWorlds()
    {
        var worldsApi = new WorldsApi(_apiConfig);
        var worlds = worldsApi.GetActiveWorlds();
        ActiveWorlds.Clear();
        foreach (var world in worlds)
        {
            var item = new CatalogWorldViewModel(world);
            ActiveWorlds.Add(item);
            Task.Run(item.LoadThumbnail);
        }
    }
    
    private void LoadFoundWorlds()
    {
        var worldsApi = new WorldsApi(_apiConfig);
        var worlds = worldsApi.SearchWorlds(search: Search, featured: Featured.ToString(), sort: Sort.Key, platform: Quest ? "android" : "");
        ActiveWorlds.Clear();
        foreach (var world in worlds)
        {
            var item = new CatalogWorldViewModel(world);
            ActiveWorlds.Add(item);
            Task.Run(item.LoadThumbnail);
        }
    }

    public ObservableCollection<CatalogWorldViewModel> ActiveWorlds { get; } = new();

    public CatalogWorldViewModel SelectedWorld
    {
        get => _selectedWorld;
        set => this.RaiseAndSetIfChanged(ref _selectedWorld, value);
    }

    public string Search
    {
        get => _search;
        set => this.RaiseAndSetIfChanged(ref _search, value);
    }
    
    public bool Featured
    {
        get => _featured;
        set => this.RaiseAndSetIfChanged(ref _featured, value);
    }
    
    public KeyValuePair<string, string> Sort
    {
        get => _sort;
        set => this.RaiseAndSetIfChanged(ref _sort, value);
    }
    
    public bool Quest
    {
        get => _quest;
        set => this.RaiseAndSetIfChanged(ref _quest, value);
    }

    public Dictionary<string, string> SortList => new()
    {
        { "popularity", "популярность" },
        { "heat", "тренд" },
        { "trust", "доверие" },
        { "shuffle", "смешанно" },
        { "random", "случайно" },
        { "favorites", "избранное" },
        { "publicationDate", "дата публикации" },
        { "labsPublicationDate", "дата публикации в лаборатории" },
        { "order", "порядок" },
        { "relevance", "релевантность" },
        { "magic", "магия" },
        { "name", "название" }
    };
}