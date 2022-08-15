using System;
using Avalonia;
using DynamicData;
using ReactiveUI;
using WorldManager.Services;
using WorldManager.ViewModels.WorldDetails;
using WorldManager.Views.WorldDetails;

namespace WorldManager.ViewModels.SavedWorlds;

public class SavedSelectionViewModel : ViewModelBase
{
    private string _newGroup;

    public SavedSelectionViewModel()
    {
        Item = new SavedWorldViewModel();
    }

    public SavedSelectionViewModel(SavedWorldViewModel item)
    {
        Item = item;
        _newGroup = item.World.Group ?? "";
    }

    public SavedWorldViewModel Item { get; }

    public string NewGroup
    {
        get => _newGroup;
        set => this.RaiseAndSetIfChanged(ref _newGroup, value);
    }

    public event Action Removed;

    public async void CopyName()
    {
        if (Application.Current is {Clipboard: { }})
            await Application.Current.Clipboard.SetTextAsync(Item.World.Name);
    }

    public void Remove()
    {
        AppCompose.DbRepository.Worlds.Remove(Item.World);
        AppCompose.DbRepository.Save();

        Removed.Invoke();
    }

    public void SetGroup()
    {
        Item.World.Group = string.IsNullOrWhiteSpace(NewGroup) ? null : NewGroup;
        AppCompose.DbRepository.Worlds.AddOrUpdate(Item.World);
        AppCompose.DbRepository.Save();
    }

    public void Details()
    {
        AppCompose.MainWindow?.SetView(new WorldView(),
            new WorldViewModel(AppCompose.MainWindow.ApiConfig, Item.World.Id));
    }
}