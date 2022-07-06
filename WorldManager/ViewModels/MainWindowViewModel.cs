using System;
using ReactiveUI;

namespace WorldManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private CatalogViewModel? _catalogViewModel;
        
        private TestViewModel? _testViewModel;

        private bool _test;

        public MainWindowViewModel()
        {
            this.WhenAnyValue(x => x.AuthViewModel.Authorized)
                .Subscribe(x =>
                {
                    CatalogViewModel = x && AuthViewModel.ApiConfig != null ? new CatalogViewModel(AuthViewModel.ApiConfig) : null;
                    TestViewModel = x && AuthViewModel.ApiConfig != null ? new TestViewModel(AuthViewModel.ApiConfig) : null;
                });
        }

        public AuthViewModel AuthViewModel { get; } = new();

        public CatalogViewModel? CatalogViewModel
        {
            get => _catalogViewModel;
            set => this.RaiseAndSetIfChanged(ref _catalogViewModel, value);
        }
        
        public TestViewModel? TestViewModel
        {
            get => _testViewModel;
            set => this.RaiseAndSetIfChanged(ref _testViewModel, value);
        }

        public bool Test
        {
            get => _test;
            set => this.RaiseAndSetIfChanged(ref _test, value);
        }
    }
}