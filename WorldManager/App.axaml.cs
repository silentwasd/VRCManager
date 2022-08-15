using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using WorldManager.Services;
using WorldManager.ViewModels;
using WorldManager.Views;

namespace WorldManager;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = new MainWindowViewModel();

            desktop.MainWindow = new MainWindow
            {
                DataContext = vm
            };

            AppCompose.MainWindow = vm;
        }

        base.OnFrameworkInitializationCompleted();
    }
}