namespace WorldManager.ViewModels.Navigation;

public class NavItem : ViewModelBase
{
    public NavItem(string name, object content, ViewModelBase context)
    {
        Name = name;
        Content = content;
        Context = context;
    }

    public string Name { get; }

    public object Content { get; }

    public ViewModelBase Context { get; }
}