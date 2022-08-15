using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WorldManager.Views.WorldDetails;

public partial class WorldView : UserControl
{
    public WorldView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}