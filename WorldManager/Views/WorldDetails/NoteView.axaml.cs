using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WorldManager.Views.WorldDetails;

public partial class NoteView : UserControl
{
    public NoteView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}