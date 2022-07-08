using System;
using Avalonia;
using WorldManager.Services;

namespace WorldManager.ViewModels;

public class SavedSelectionViewModel : ViewModelBase
{
    public event Action Removed;
    
    public SavedSelectionViewModel()
    {
        Item = new SavedWorldViewModel();
    }
    
    public SavedSelectionViewModel(SavedWorldViewModel item)
    {
        Item = item;
    }

    public SavedWorldViewModel Item { get; }

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
}