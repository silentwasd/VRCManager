using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using VRChat.API.Api;
using VRChat.API.Client;
using WorldManager.Services;

namespace WorldManager.ViewModels;

public class SavedWorldsViewModel : ViewModelBase
{
    private readonly WorldsApi _worldsApi;
    
    private SavedWorldViewModel? _selectedWorld;
    
    private SavedSelectionViewModel? _selection;

    public SavedWorldsViewModel()
    {
        _worldsApi = new WorldsApi(new Configuration());
        Items.AddRange(new []
        {
            new SavedWorldViewModel(),
            new SavedWorldViewModel(),
            new SavedWorldViewModel()
        });
    }
    
    public SavedWorldsViewModel(Configuration apiConfig)
    {
        _worldsApi = new WorldsApi(apiConfig);
        
        this.WhenAnyValue(x => x.SelectedWorld)
            .Subscribe(x =>
            {
                if (x == null)
                {
                    Selection = null;
                    return;
                }

                Selection = new(x);
                Selection.Removed += () =>
                {
                    Items.Remove(SelectedWorld);
                    SelectedWorld = null;
                };
            });

        LoadWorlds();
    }
    
    public ObservableCollection<SavedWorldViewModel> Items { get; } = new();

    public SavedWorldViewModel? SelectedWorld
    {
        get => _selectedWorld;
        set => this.RaiseAndSetIfChanged(ref _selectedWorld, value);
    }
    
    public SavedSelectionViewModel? Selection
    {
        get => _selection;
        set => this.RaiseAndSetIfChanged(ref _selection, value);
    }

    private void LoadWorlds()
    {
        Items.Clear();
        
        foreach (var world in AppCompose.DbRepository.Worlds)
            Items.Add(new SavedWorldViewModel(world));
        
        LoadWorldThumbnails();
    }

    private void LoadWorldThumbnails()
    {
        foreach (var item in Items)
            Task.Run(item.LoadThumbnail);
    }

    public override void NavTrigger()
    {
        LoadWorlds();
    }
}