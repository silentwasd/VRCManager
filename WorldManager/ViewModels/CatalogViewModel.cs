using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

    public CatalogViewModel()
    {
        _apiConfig = new Configuration();
    }
    
    public CatalogViewModel(Configuration apiConfig)
    {
        _apiConfig = apiConfig;
        LoadActiveWorlds();

        this.WhenAnyValue(x => x.Search)
            .Throttle(TimeSpan.FromSeconds(1))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x =>
            {
                if (String.IsNullOrWhiteSpace(x))
                    LoadActiveWorlds();
                else
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
        var worlds = worldsApi.SearchWorlds(search: Search);
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
}