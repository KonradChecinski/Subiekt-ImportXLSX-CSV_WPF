using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    /// <summary>
    /// Logika interakcji dla klasy Settings.xaml
    /// </summary>
    public partial class SettingsControll : UserControl
    {
        public SettingsControll()
        {
            DataContext = new SettingsViewModel();

            InitializeComponent();
            this.Loaded += Window_Loaded; // Obsługuje zdarzenie załadowania okna
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Pobierz ViewModel
            var viewModel = DataContext as SettingsViewModel;
            if (viewModel != null)
            {
                // Pobieramy PasswordBox z XAML
                var passwordBox = this.FindName("PasswordBox") as PasswordBox;
                if (passwordBox != null)
                {
                    // Ustaw hasło w PasswordBox
                    passwordBox.Password = viewModel.Password;
                }
            }
        }

        // Obsługuje zdarzenie PasswordChanged
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Pobieramy PasswordBox z XAML
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                // Wywołujemy metodę w ViewModelu i przekazujemy nowe hasło
                var viewModel = DataContext as SettingsViewModel;
                if (viewModel != null)
                {
                    Debug.WriteLine("PasswordBox_PasswordChanged: " + passwordBox.Password);
                    viewModel.Password = passwordBox.Password;
                }
            }
        }
    }
}
