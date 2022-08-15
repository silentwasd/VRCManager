using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WorldManager.Views.SavedWorlds;

public partial class SavedWorldsView : UserControl
{
    public SavedWorldsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}