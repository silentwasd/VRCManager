using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;

namespace WorldManager.ViewModels;

public class CatalogViewModel : ViewModelBase
{
    private const int Count = 50;

    private readonly WorldsApi _worldsApi;

    private CatalogWorldViewModel? _selectedWorld;

    private string _search = "";

    private KeyValuePair<string, string> _sort;

    private bool _quest;

    private bool _featured;

    private int _offset;

    private bool _loading;

    private int _counter;

    public CatalogViewModel()
    {
        _worldsApi = new WorldsApi(new Configuration());
        Reset = ReactiveCommand.Create(() => {});
        Back = ReactiveCommand.Create(() => {});
        Next = ReactiveCommand.Create(() => {});
    }
    
    public CatalogViewModel(Configuration apiConfig)
    {
        _worldsApi = new WorldsApi(apiConfig);

        Sort = new KeyValuePair<string, string>("popularity", "популярность");
        Quest = false;

        this.WhenAnyValue(x => x.Search, x => x.Featured, x => x.Sort, x => x.Quest,
                (_, _, sort, _) => sort.Key != String.Empty)
            .Throttle(TimeSpan.FromSeconds(1))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
            {
                Offset = 0;
                LoadFoundWorlds();
            });

        var backAvailable = this.WhenAnyValue(x => x.Offset,
            x => x > 0);

        var nextAvailable = this.WhenAnyValue(
            x => x.ActiveWorlds.Count, x => x.Offset,
            (count, offset) => count == Count && offset + Count < 1000
        );

        Reset = ReactiveCommand.Create(() =>
        {
            Offset = 0;
            LoadFoundWorlds();
        });
        
        Back = ReactiveCommand.Create(() =>
        {
            Offset -= Count;
            LoadFoundWorlds();
        }, backAvailable);
        
        Next = ReactiveCommand.Create(() =>
        {
            Offset += Count;
            LoadFoundWorlds();
        }, nextAvailable);
    }
    
    private async void LoadFoundWorlds()
    {
        Loading = true;
        
        List<LimitedWorld>? worlds = await _worldsApi.SearchWorldsAsync(search: Search, featured: Featured ? "True" : null,
            sort: Sort.Key, platform: Quest ? "android" : "", n: Count, offset: Offset);
        ActiveWorlds.Clear();

        if (worlds == null)
        {
            Loading = false;
            return;
        }

        foreach (var world in worlds)
        {
            var item = new CatalogWorldViewModel(world);
            ActiveWorlds.Add(item);
        }

        Loading = false;
        
        LoadWorldThumbnails();
    }

    private void LoadWorldThumbnails()
    {
        foreach (var item in ActiveWorlds)
        {
            Task.Run(item.LoadThumbnail);
        }
    }

    public ObservableCollection<CatalogWorldViewModel> ActiveWorlds { get; } = new();

    public CatalogWorldViewModel? SelectedWorld
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

    private int Offset
    {
        get => _offset;
        set => this.RaiseAndSetIfChanged(ref _offset, value);
    }

    public bool Loading
    {
        get => _loading;
        set => this.RaiseAndSetIfChanged(ref _loading, value);
    }

    public int Counter
    {
        get => _counter;
        set => this.RaiseAndSetIfChanged(ref _counter, value);
    }

    public ReactiveCommand<Unit, Unit> Reset { get; }
    
    public ReactiveCommand<Unit, Unit> Back { get; }

    public ReactiveCommand<Unit, Unit> Next { get; }
}