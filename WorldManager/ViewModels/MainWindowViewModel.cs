using System;
using ReactiveUI;

namespace WorldManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private AuthViewModel _authViewModel = new();

        public AuthViewModel AuthViewModel => _authViewModel;
    }
}