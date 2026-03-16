using FlowerShop.Pages.AdminPanel;
using System.Windows;
using System.Windows.Controls;


namespace FlowerShop.Pages
{
    public partial class AdminPanelPage : Page
    {
        public AdminPanelPage()
        {
            InitializeComponent();
            LoadTxtBox();
        }

        private void LoadTxtBox()
        {
            TxtAdmin.Text = $"Админ-панель";
        }

        private void BtnAdminFlowers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminFlowersPage());
        }

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCategoriesPage());
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
            MessageBox.Show("Корзина пока в разработке", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Профиль пока в разработке", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminAddFlowersPage());
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminClientsPage());
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