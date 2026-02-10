using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FlowerShop.Pages
{
    public partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            InitializeComponent();
            TxtWelcome.Text = $"Добро пожаловать, {App.CurrentUser?.Username}!";
        }

        private void BtnFlowers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FlowersCatalogPage());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            NavigationService.Navigate(new AuthPage());
        }
    }
}