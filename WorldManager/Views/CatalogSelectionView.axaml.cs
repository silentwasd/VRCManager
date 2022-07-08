using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
}