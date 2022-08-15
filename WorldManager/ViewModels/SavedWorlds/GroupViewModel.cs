using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;
using WorldManager.Services.Db;

namespace WorldManager.ViewModels.SavedWorlds;

public class GroupViewModel : ViewModelBase
{
    private readonly ReadOnlyObservableCollection<SavedWorldViewModel> _worlds;

    public GroupViewModel()
    {
        Group = "Test Group";

        _worlds = new ReadOnlyObservableCollection<SavedWorldViewModel>(new ObservableCollection<SavedWorldViewModel>(
            new[]
            {
                new SavedWorldViewModel(),
                new SavedWorldViewModel(),
                new SavedWorldViewModel()
            }));
    }

    public GroupViewModel(IGroup<World, string, string?> group, SavedWorldsViewModel parent)
    {
        Group = group.Key == "_" ? "Без группы" : group.Key;
        Parent = parent;

        group.Cache.Connect()
            .Transform(x =>
            {
                x.Group = x.Group == "_" ? null : x.Group;
                return x;
            })
            .Where(group.Key == "_" ? x => x.Group == null : x => x.Group == Group)
            .Transform(x => new SavedWorldViewModel(x, Parent))
            .ForEachChange(x =>
            {
                if (x.Current.Thumbnail == null)
                    Task.Run(x.Current.LoadThumbnail);
            })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _worlds)
            .DisposeMany()
            .Subscribe();
    }

    public string Group { get; }

    public ReadOnlyObservableCollection<SavedWorldViewModel> Worlds => _worlds;

    private SavedWorldsViewModel Parent { get; }

    public event Action<SavedWorldViewModel> WorldSelected;

    public void SelectWorld(SavedWorldViewModel world)
    {
        WorldSelected?.Invoke(world);
    }
}