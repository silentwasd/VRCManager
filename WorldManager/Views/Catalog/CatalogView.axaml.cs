using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WorldManager.Views.Catalog;

public partial class CatalogView : UserControl
{
    public CatalogView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}