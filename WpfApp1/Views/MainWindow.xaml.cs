using MaterialDesignThemes.Wpf;
using System.Windows;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var paletteHelper = new PaletteHelper();
            //Retrieve the app's existing theme
            Theme theme = paletteHelper.GetTheme();

            //Change the base theme to Dark
            theme.SetBaseTheme(BaseTheme.Dark);
            //or theme.SetBaseTheme(Theme.Light);

            //Change all of the primary colors to Red
            //theme.SetPrimaryColor(System.Windows.Media.Color.FromRgb(228, 230, 238));
            theme.SetPrimaryColor(System.Windows.Media.Color.FromRgb(67, 100, 156));

            //Change all of the secondary colors to Blue
            theme.SetSecondaryColor(System.Windows.Media.Color.FromRgb(0, 114, 186));

            //Change the app's current theme
            paletteHelper.SetTheme(theme);

            InitializeComponent();
            // Inicjalizacja ViewModel
            var viewModel = new MainViewModel();
            DataContext = viewModel;


        }
    }
}