using FlowerShop.Pages.AdminPanel;
using System.Windows;
using System.Windows.Controls;

namespace FlowerShop.Pages
{
    public partial class AdminPanelPage : Page
    {
        // Конструктор страницы
        public AdminPanelPage()
        {
            InitializeComponent();
            LoadTxtBox();
        }

        // Загрузка текста приветствия
        private void LoadTxtBox()
        {
            TxtAdmin.Text = "Админ-панель";
        }

        // Переход к управлению заказами
        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminOrdersPage());
        }

        // Переход к управлению товарами
        private void BtnAdminFlowers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminFlowersPage());
        }

        // Переход к управлению категориями
        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCategoriesPage());
        }

        // Переход к управлению пользователями
        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminClientsPage());
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

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }

        // Выход из аккаунта
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            NavigationService.Navigate(new AuthPage());
        }
    }
}