using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using WorldManager.ViewModels;

namespace WorldManager.Views;

public partial class GroupView : UserControl
{
    public GroupView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnGroupSelected(object? sender, PointerPressedEventArgs e)
    {
        var border = (Border) sender!;
        var view = (SavedWorldView) border.Child;
        var savedWorld = (SavedWorldViewModel) view.DataContext!;

        var group = (GroupViewModel) DataContext!;
        group.SelectWorld(savedWorld);
    }
}