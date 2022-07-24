using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WorldManager.ViewModels;

namespace WorldManager.Views;

public partial class CatalogSelectionView : UserControl
{
    public CatalogSelectionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnGroupSelected(object? sender, SelectionChangedEventArgs e)
    {
        var list = (ListBox) sender!;
        var vm = (CatalogSelectionViewModel) list.DataContext!;

        if (list.SelectedItem == null)
            return;

        var item = (string) list.SelectedItem;
        vm.Group = item;
    }
}