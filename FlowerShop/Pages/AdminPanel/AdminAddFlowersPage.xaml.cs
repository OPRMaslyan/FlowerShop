using FlowerShop.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FlowerShop.Pages.AdminPanel
{
    public partial class AdminAddFlowersPage : Page
    {
        private byte[] _imageData;

        public AdminAddFlowersPage()
        {
            InitializeComponent();
            LoadCategories();
        }

        // Загрузка категорий из БД
        private void LoadCategories()
        {
            using (var context = new FlowerShopDbContext())
            {
                var categories = context.Categories.ToList();
                ComboBoxCategories.ItemsSource = categories;
                ComboBoxCategories.DisplayMemberPath = "Name";
                ComboBoxCategories.SelectedValuePath = "Id";

                if (categories.Any())
                {
                    ComboBoxCategories.SelectedIndex = 0;
                }
            }
        }

        // Выбор изображения
        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp|Все файлы|*.*",
                Title = "Выберите изображение товара"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Чтение файла в байты
                    _imageData = File.ReadAllBytes(openFileDialog.FileName);

                    // Предпросмотр
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(_imageData);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    ImagePreview.Source = bitmap;

                    // Имя файла
                    TxtImageName.Text = Path.GetFileName(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Только цифры для цены и количества
        private void TBoxPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        // Сохранение товара
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Проверка полей
            if (string.IsNullOrWhiteSpace(TBoxName.Text))
            {
                ShowError("Введите название товара!");
                return;
            }

            if (string.IsNullOrWhiteSpace(TBoxDescription.Text))
            {
                ShowError("Введите описание товара!");
                return;
            }

            if (!decimal.TryParse(TBoxPrice.Text, out decimal price) || price <= 0)
            {
                ShowError("Введите корректную цену!");
                return;
            }

            if (!int.TryParse(TBoxStock.Text, out int stock) || stock < 0)
            {
                ShowError("Введите корректное количество!");
                return;
            }

            if (ComboBoxCategories.SelectedItem == null)
            {
                ShowError("Выберите категорию!");
                return;
            }

            try
            {
                using (var context = new FlowerShopDbContext())
                {
                    var flower = new Flower
                    {
                        Name = TBoxName.Text.Trim(),
                        Description = TBoxDescription.Text.Trim(),
                        Price = price,
                        Stockquantity = stock,
                        Categoryid = ((Category)ComboBoxCategories.SelectedItem).Id,
                        ImageData = _imageData ?? null
                    };

                    context.Flowers.Add(flower);
                    context.SaveChanges();

                }

                MessageBox.Show("Товар успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
                NavigationService.Navigate(new AdminFlowersPage());
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка сохранения: {ex.Message}");
            }
        }

        // Очистка формы
        private void ClearForm()
        {
            TBoxName.Clear();
            TBoxDescription.Clear();
            TBoxPrice.Clear();
            TBoxStock.Clear();
            ImagePreview.Source = null;
            _imageData = null;
            TxtImageName.Text = "Файл не выбран";
            ComboBoxCategories.SelectedIndex = 0;
        }

        // Отмена
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Отменить изменения?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                NavigationService.Navigate(new MainMenuPage());
            }
        }

        // Навигация
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AboutPage());
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Корзина в разработке", "Информация");
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Профиль в разработке", "Информация");

        // Показ ошибок/успеха
        private void ShowError(string message)
        {
            TxtError.Text = message;
            TxtError.Visibility = Visibility.Visible;
            TxtSuccess.Visibility = Visibility.Collapsed;
        }

        private void ComboBoxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}