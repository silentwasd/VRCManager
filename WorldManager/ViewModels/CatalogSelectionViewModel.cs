using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;
using WorldManager.Services;
using WorldManager.Services.Db;

namespace WorldManager.ViewModels;

public class CatalogSelectionViewModel : ViewModelBase
{
    private readonly ObservableAsPropertyHelper<bool> _isGroupsVisible;

    private string _group;

    private ReadOnlyObservableCollection<string> _groups;
    private bool _saved;

    public CatalogSelectionViewModel()
    {
        Item = new CatalogWorldViewModel();

        _groups = new ReadOnlyObservableCollection<string>(new ObservableCollection<string>(new[]
        {
            "Test",
            "Other test",
            "Another test"
        }));
    }

    public CatalogSelectionViewModel(CatalogWorldViewModel item)
    {
        Item = item;

        ReadOnlyObservableCollection<World> worlds;

        AppCompose.DbRepository.Worlds.Connect()
            .Where(x => x.Id == Item.World.Id)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out worlds)
            .DisposeMany()
            .Subscribe();

        AppCompose.DbRepository.Groups.Connect()
            .FilterOnObservable(group =>
                this.WhenAnyValue(x => x.Group).Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => group.ToLower().Contains(x.ToLower())).Throttle(TimeSpan.FromMilliseconds(100))
                    .ObserveOn(RxApp.MainThreadScheduler))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _groups)
            .DisposeMany()
            .Subscribe();

        worlds.WhenAnyValue(x => x.Count)
            .Subscribe(x => { Saved = x > 0; });

        _isGroupsVisible = this.WhenAnyValue(x => x.Group, x => x.Groups.Count)
            .Select(x => !string.IsNullOrWhiteSpace(x.Item1) && x.Item2 > 0)
            .ToProperty(this, x => x.IsGroupsVisible);
    }

    public CatalogWorldViewModel Item { get; }

    public ReadOnlyObservableCollection<string> Groups => _groups;

    public string Group
    {
        get => _group;
        set => this.RaiseAndSetIfChanged(ref _group, value);
    }

    public bool IsGroupsVisible => _isGroupsVisible.Value;

    public bool Saved
    {
        get => _saved;
        set => this.RaiseAndSetIfChanged(ref _saved, value);
    }

    public async void CopyName()
    {
        if (Application.Current is {Clipboard: { }})
            await Application.Current.Clipboard.SetTextAsync(Item.World.Name);
    }

    public void SaveWorld()
    {
        var world = World.FromLimited(Item.World);
        world.Group = string.IsNullOrWhiteSpace(Group) ? null : Group;

        AppCompose.DbRepository.Worlds.AddOrUpdate(world);

        if (!string.IsNullOrWhiteSpace(Group) && !AppCompose.DbRepository.Groups.Items.Contains(Group))
            AppCompose.DbRepository.Groups.Add(Group);

        AppCompose.DbRepository.Save();

        Saved = true;
    }
}