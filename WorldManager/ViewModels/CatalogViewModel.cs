using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using ReactiveUI;
using VRChat.API.Api;
using VRChat.API.Client;

namespace WorldManager.ViewModels;

public class CatalogViewModel : ViewModelBase
{
    private Configuration _apiConfig;

    private CatalogWorldViewModel _selectedWorld;

    public CatalogViewModel()
    {
        _apiConfig = new Configuration();
    }
    
    public CatalogViewModel(Configuration apiConfig)
    {
        _apiConfig = apiConfig;
        LoadActiveWorlds();
    }

    private void LoadActiveWorlds()
    {
        var worldsApi = new WorldsApi(_apiConfig);
        var worlds = worldsApi.GetActiveWorlds();
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
}