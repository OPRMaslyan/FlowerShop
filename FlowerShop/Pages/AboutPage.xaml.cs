using System.Windows;
using System.Windows.Controls;

namespace FlowerShop.Pages
{
    public partial class AboutPage : Page
    {
        // Конструктор страницы
        public AboutPage()
        {
            InitializeComponent();
            // Карта загружается напрямую из XAML через WebView2.Source
        }

        // Навигация по меню
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) { }
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new CartPage());
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ProfilePage());
    }
}