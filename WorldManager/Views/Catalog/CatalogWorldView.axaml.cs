using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WorldManager.Views.Catalog;

public partial class CatalogWorldView : UserControl
{
    public CatalogWorldView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}