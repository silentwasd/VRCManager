using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace WorldManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ObservableAsPropertyHelper<bool> _isAuthView;

        private readonly ObservableAsPropertyHelper<bool> _isCatalogView;

        private readonly ObservableAsPropertyHelper<bool> _isSavedView;

        private readonly ObservableAsPropertyHelper<bool> _isTestView;
        private AuthViewModel _authViewModel = new();

        private CatalogViewModel? _catalogViewModel;

        private string _currentView = "";

        private SavedWorldsViewModel? _savedWorldsViewModel;

        private TestViewModel? _testViewModel;

        public MainWindowViewModel()
        {
            CurrentView = "auth";

            this.WhenAnyValue(x => x.AuthViewModel.Authorized)
                .Subscribe(x =>
                {
                    CatalogViewModel = x && AuthViewModel.ApiConfig != null
                        ? new CatalogViewModel(AuthViewModel.ApiConfig)
                        : null;
                    SavedWorldsViewModel = x && AuthViewModel.ApiConfig != null
                        ? new SavedWorldsViewModel(AuthViewModel.ApiConfig)
                        : null;
                    TestViewModel = x && AuthViewModel.ApiConfig != null
                        ? new TestViewModel(AuthViewModel.ApiConfig)
                        : null;

                    if (CatalogViewModel != null)
                        CurrentView = "catalog";
                });

            _isAuthView = this
                .WhenAnyValue(x => x.CurrentView)
                .Select(x => x == "auth")
                .ToProperty(this, x => x.IsAuthView);

            _isCatalogView = this
                .WhenAnyValue(x => x.CurrentView)
                .Select(x => x == "catalog")
                .ToProperty(this, x => x.IsCatalogView);

            _isSavedView = this
                .WhenAnyValue(x => x.CurrentView)
                .Select(x => x == "saved")
                .ToProperty(this, x => x.IsSavedView);

            _isTestView = this
                .WhenAnyValue(x => x.CurrentView)
                .Select(x => x == "test")
                .ToProperty(this, x => x.IsTestView);
        }

        public AuthViewModel AuthViewModel
        {
            get => _authViewModel;
            set => this.RaiseAndSetIfChanged(ref _authViewModel, value);
        }

        public CatalogViewModel? CatalogViewModel
        {
            get => _catalogViewModel;
            set => this.RaiseAndSetIfChanged(ref _catalogViewModel, value);
        }

        public SavedWorldsViewModel? SavedWorldsViewModel
        {
            get => _savedWorldsViewModel;
            set => this.RaiseAndSetIfChanged(ref _savedWorldsViewModel, value);
        }

        public TestViewModel? TestViewModel
        {
            get => _testViewModel;
            set => this.RaiseAndSetIfChanged(ref _testViewModel, value);
        }

        private string CurrentView
        {
            get => _currentView;
            set => this.RaiseAndSetIfChanged(ref _currentView, value);
        }

        private ViewModelBase CurrentViewModel
        {
            get
            {
                switch (CurrentView)
                {
                    case "catalog":
                        return CatalogViewModel;

                    case "saved":
                        return SavedWorldsViewModel;

                    case "test":
                        return TestViewModel;

                    default:
                        return AuthViewModel;
                }
            }
        }

        public bool IsAuthView => _isAuthView.Value;

        public bool IsCatalogView => _isCatalogView.Value;

        public bool IsSavedView => _isSavedView.Value;

        public bool IsTestView => _isTestView.Value;

        public void SelectView(string view)
        {
            CurrentView = view;
            CurrentViewModel.NavTrigger();
        }
    }
}