using System.Windows;
using System.Windows.Controls;

namespace FlowerShop.Pages
{
    public partial class MainMenuPage : Page
    {
        // Конструктор страницы
        public MainMenuPage()
        {
            InitializeComponent();
            LoadUserData();
        }

        // Загрузка данных пользователя
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

        // Переход к каталогу цветов
        private void BtnFlowers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FlowersCatalogPage());
        }

        // Навигация по меню
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