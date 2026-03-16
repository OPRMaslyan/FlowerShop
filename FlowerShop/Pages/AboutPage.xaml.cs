using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FlowerShop.Pages
{
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
            Loaded += MainMap_Loaded;
        }

        private async void MainMap_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await WebViewMap.EnsureCoreWebView2Async(null);

                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string mapPath = Path.Combine(appPath, "Web", "map.html");

                if (File.Exists(mapPath))
                {
                    WebViewMap.Source = new Uri(mapPath);
                }
                else
                {
                    string errorHtml = "<html><body style='font-family: Arial; text-align: center; padding: 20px;'>" +
                                      "<h3>Файл карты не найден</h3>" +
                                      $"<p>Путь: {mapPath}</p>" +
                                      "</body></html>";
                    WebViewMap.NavigateToString(errorHtml);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки карты: {ex.Message}");
            }
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
            MessageBox.Show("Как нибудь потом", "Будущее", MessageBoxButton.OKCancel);
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProfilePage());
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }
    }
}