using System;
using ReactiveUI;

namespace WorldManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private CatalogViewModel? _catalogViewModel;

        public MainWindowViewModel()
        {
            this.WhenAnyValue(x => x.AuthViewModel.Authorized)
                .Subscribe(x => CatalogViewModel = x ? new CatalogViewModel(AuthViewModel.ApiConfig) : null);
        }

        public AuthViewModel AuthViewModel { get; } = new();

        public CatalogViewModel? CatalogViewModel
        {
            get => _catalogViewModel;
            set => this.RaiseAndSetIfChanged(ref _catalogViewModel, value);
        }
    }
}