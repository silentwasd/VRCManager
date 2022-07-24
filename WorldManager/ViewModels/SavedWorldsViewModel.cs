using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using VRChat.API.Api;
using VRChat.API.Client;
using WorldManager.Services;

namespace WorldManager.ViewModels;

public class SavedWorldsViewModel : ViewModelBase
{
    private readonly ReadOnlyObservableCollection<GroupViewModel> _groups;
    private readonly WorldsApi _worldsApi;

    private SavedWorldViewModel? _selectedWorld;

    private SavedSelectionViewModel? _selection;

    public SavedWorldsViewModel()
    {
        _worldsApi = new WorldsApi(new Configuration());

        _groups = new ReadOnlyObservableCollection<GroupViewModel>(new ObservableCollection<GroupViewModel>(new[]
        {
            new GroupViewModel(),
            new GroupViewModel(),
            new GroupViewModel()
        }));
    }

    public SavedWorldsViewModel(Configuration apiConfig)
    {
        _worldsApi = new WorldsApi(apiConfig);

        AppCompose.DbRepository.Worlds
            .Connect()
            .Transform(x =>
            {
                x.Group ??= "_";
                return x;
            })
            .Group(x => x.Group ?? "_")
            .Transform(x =>
            {
                var group = new GroupViewModel(x, this);
                group.WorldSelected += (world) => SelectedWorld = world;
                return group;
            })
            .Sort(SortExpressionComparer<GroupViewModel>.Ascending(x => x.Group))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _groups)
            .DisposeMany()
            .Subscribe();

        this.WhenAnyValue(x => x.SelectedWorld)
            .Subscribe(x =>
            {
                if (x == null)
                {
                    Selection = null;
                    return;
                }

                Selection = new(x);
                Selection.Removed += () => { SelectedWorld = null; };
            });
    }

    public ReadOnlyObservableCollection<GroupViewModel> Groups => _groups;

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
}