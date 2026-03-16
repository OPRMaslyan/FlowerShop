using System.Windows;
using System.Windows.Controls;

namespace FlowerShop.Pages
{
    public partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            TxtWelcome.Text = $"Добро пожаловать, {App.CurrentUser?.Username}!";

            if (App.CurrentUser?.Role == "Admin")
            {
                AdminPanel.Visibility = Visibility.Visible;
            }
            else
            {
                AdminPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnFlowers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FlowersCatalogPage());
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FlowersCatalogPage());
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AboutPage());
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage());
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProfilePage());
        }

        private void BtnAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminPanelPage());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            NavigationService.Navigate(new AuthPage());
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }
    }
}