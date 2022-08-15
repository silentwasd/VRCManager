using Avalonia.Media;
using ReactiveUI;

namespace WorldManager.ViewModels.WorldDetails;

public class TagViewModel : ViewModelBase
{
    private IBrush _color = Brushes.Transparent;

    private string _hint;
    private string _name;

    public TagViewModel(string name)
    {
        Name = FormatName(name);
        Color = SelectColor(name);
        Hint = SelectHint(name);
    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public IBrush Color
    {
        get => _color;
        set => this.RaiseAndSetIfChanged(ref _color, value);
    }

    public string Hint
    {
        get => _hint;
        set => this.RaiseAndSetIfChanged(ref _hint, value);
    }

    private string FormatName(string raw)
    {
        if (raw.StartsWith("author_tag_")) return ReplaceUnderscore(UpperFirst(raw.Substring("author_tag_".Length)));

        if (raw.StartsWith("system_")) return ReplaceUnderscore(UpperFirst(raw.Substring("system_".Length)));

        if (raw.StartsWith("admin_")) return ReplaceUnderscore(UpperFirst(raw.Substring("admin_".Length)));

        return raw;
    }

    private IBrush SelectColor(string raw)
    {
        if (raw.StartsWith("author_tag_")) return Brushes.DodgerBlue;

        if (raw.StartsWith("system_")) return Brushes.Orange;

        if (raw.StartsWith("admin_")) return Brushes.DarkGreen;

        return Brushes.Transparent;
    }

    private string SelectHint(string raw)
    {
        if (raw.StartsWith("author_tag_")) return "Авторский тег";

        if (raw.StartsWith("system_")) return "Системный тег";

        if (raw.StartsWith("admin_")) return "Тег администратора";

        return raw;
    }

    private string UpperFirst(string raw)
    {
        if (raw.Length > 1)
            return raw.Substring(0, 1).ToUpper() + raw.Substring(1);

        return raw.ToUpper();
    }

    private string ReplaceUnderscore(string raw)
    {
        return raw.Replace('_', ' ');
    }
}