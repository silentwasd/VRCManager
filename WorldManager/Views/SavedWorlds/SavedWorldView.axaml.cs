using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WorldManager.Views.SavedWorlds;

public partial class SavedWorldView : UserControl
{
    public SavedWorldView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}