using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FlowerShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        // Конструктор страницы
        public AboutPage()
        {
            InitializeComponent();
            Loaded += MainMap_Loaded;
        }

        // Загрузка карты в WebView2
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

        // Навигация к каталогу
        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FlowersCatalogPage());
        }

        // Навигация на страницу "О нас" (уже здесь)
        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            // Уже на странице, ничего не делаем
        }

        // Навигация в корзину
        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage());
        }

        // Навигация в профиль
        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProfilePage());
        }

        // Навигация в главное меню
        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }
    }
}