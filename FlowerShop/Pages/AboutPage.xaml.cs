using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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

namespace FlowerShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
            Loaded += MainMap_Loaded;
        }

        // Всего яндекс позволяет 1000 раз бесплатно в день подключаться к API 
        private async void MainMap_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Инициализация WebView асинхронно
                await WebViewMap.EnsureCoreWebView2Async(null);

                // Корень проекта
                string appPath = AppDomain.CurrentDomain.BaseDirectory;

                // Путь к карте
                string mapPath = Path.Combine(appPath, "Web", "map.html");


                if (File.Exists(mapPath))
                {
                    // Отрисовываем карту
                    WebViewMap.Source = new Uri(mapPath);
                }
                else
                {
                    // Отрисовываем ошибку (если карты нет)
                    string errorHtml = "<html><body style='font-family: Arial; text-align: center; padding: 20px;'>" +
                                      "<h3>Файл карты не найден</h3>" +
                                      $"<p>Путь: {mapPath}</p>" +
                                      "</body></html>";
                    WebViewMap.NavigateToString(errorHtml);
                }
            }
            catch (Exception ex)
            {
                // Отрисовываем ошибку (если не получилось загрузить)
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
            MessageBox.Show("Как нибудь потом", "Будущее", MessageBoxButton.OKCancel);
        }
    }
}
