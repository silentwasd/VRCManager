using System;
using System.Linq;
using ReactiveUI;
using VRChat.API.Client;
using WorldManager.ViewModels.Catalog;
using WorldManager.ViewModels.Navigation;
using WorldManager.ViewModels.SavedWorlds;
using WorldManager.Views;
using WorldManager.Views.Catalog;
using WorldManager.Views.SavedWorlds;

namespace WorldManager.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private Configuration? _apiConfig;
    private AuthViewModel _authViewModel = new();

    private object _currentView;

    private ViewModelBase _currentViewContext;

    private NavItem[] _navItems;

    public MainWindowViewModel()
    {
        this.WhenAnyValue(x => x.AuthViewModel.Authorized)
            .Subscribe(x =>
            {
                if (AuthViewModel.ApiConfig == null)
                    return;

                ApiConfig = AuthViewModel.ApiConfig;

                NavItems = new[]
                {
                    new NavItem("Каталог", new CatalogView(), new CatalogViewModel(ApiConfig)),
                    new NavItem("Сохраненные миры", new SavedWorldsView(), new SavedWorldsViewModel(ApiConfig)),
                    new NavItem("Тест", new TestView(), new TestViewModel(ApiConfig))
                };

                SelectView("Каталог");
            });
    }

    public AuthViewModel AuthViewModel
    {
        get => _authViewModel;
        set => this.RaiseAndSetIfChanged(ref _authViewModel, value);
    }

    private object CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    private ViewModelBase CurrentViewContext
    {
        get => _currentViewContext;
        set => this.RaiseAndSetIfChanged(ref _currentViewContext, value);
    }

    private NavItem[] NavItems
    {
        get => _navItems;
        set => this.RaiseAndSetIfChanged(ref _navItems, value);
    }

    public Configuration ApiConfig
    {
        get => _apiConfig;
        set => this.RaiseAndSetIfChanged(ref _apiConfig, value);
    }

    private void SelectView(string view)
    {
        var navItem = NavItems.First(nav => nav.Name == view);

        SetView(navItem.Content, navItem.Context);
    }

    public void SetView(object view, ViewModelBase context)
    {
        CurrentView = view;
        CurrentViewContext = context;
    }
}