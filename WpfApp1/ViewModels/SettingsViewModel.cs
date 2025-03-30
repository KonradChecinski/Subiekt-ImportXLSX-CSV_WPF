using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp1.Commands;

namespace WpfApp1.ViewModels
{
    internal class SettingsViewModel: BaseViewModel
    {
        private string _host;
        private string _databaseName;
        private string _username;
        private string _password;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Host
        {
            get => _host;
            set
            {
                if (_host != value)
                {
                    _host = value;
                    OnPropertyChanged(nameof(Host));
                    Properties.Settings.Default.Host = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public string DatabaseName
        {
            get => _databaseName;
            set
            {
                if (_databaseName != value)
                {
                    _databaseName = value;
                    OnPropertyChanged(nameof(DatabaseName));
                    Properties.Settings.Default.DatabaseName = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                    Properties.Settings.Default.Username = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                    Properties.Settings.Default.Password = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SettingsViewModel()
        {
            // Załaduj ustawienia z pliku ustawień aplikacji
            Host = Properties.Settings.Default.Host;
            DatabaseName = Properties.Settings.Default.DatabaseName;
            Username = Properties.Settings.Default.Username;
            Password = Properties.Settings.Default.Password;

            // Inicjalizujemy komendę
            ExecuteQueryCommand = new RelayCommand(ExecuteTestQuery);
        }


        public ICommand ExecuteQueryCommand { get; }

        // Metoda do wykonania testowego zapytania
        private void ExecuteTestQuery(object parameter)
        {
            try
            {
                string connectionString = $"Server={Host};Database={DatabaseName};User Id={Username};Password={Password};TrustServerCertificate=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT 1"; // Testowe zapytanie
                    SqlCommand command = new SqlCommand(query, connection);
                    var result = command.ExecuteScalar();
                    MessageBox.Show("Testowe zapytanie zakończone sukcesem!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wykonywania zapytania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
