using System.Windows.Controls;
using System.Windows.Input;
using WpfApp1.Commands;
using WpfApp1.Views;

namespace WpfApp1.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand ShowImportViewCommand { get; set; }
        public ICommand ShowSettingsViewCommand { get; set; }

        public MainViewModel()
        {
            ShowImportViewCommand = new RelayCommand(ShowImportView);
            ShowSettingsViewCommand = new RelayCommand(ShowSettingsView);
        }

        private void ShowImportView(object parameter)
        {
            // Załaduj UserControl ImportView
            CurrentView = new ImportControll();
        }
        private void ShowSettingsView(object parameter)
        {
            // Załaduj UserControl SettingsView
            CurrentView = new SettingsControll();

        }
    }
}
