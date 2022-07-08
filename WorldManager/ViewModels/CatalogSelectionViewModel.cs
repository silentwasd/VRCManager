using Avalonia;
using ReactiveUI;
using WorldManager.Services;
using World = WorldManager.Services.Db.World;

namespace WorldManager.ViewModels;

public class CatalogSelectionViewModel : ViewModelBase
{
    private bool _saved;
    
    public CatalogSelectionViewModel()
    {
        Item = new CatalogWorldViewModel();
    }
    
    public CatalogSelectionViewModel(CatalogWorldViewModel item)
    {
        Item = item;
        Saved = AppCompose.DbRepository.Worlds.Find(x => x.Id == Item.World.Id) != null;
    }

    public CatalogWorldViewModel Item { get; }

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
        AppCompose.DbRepository.Worlds.Add(World.FromLimited(Item.World));
        
        AppCompose.DbRepository.Save();

        Saved = true;
    }
}