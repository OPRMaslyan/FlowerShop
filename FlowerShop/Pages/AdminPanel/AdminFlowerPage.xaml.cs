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
    public partial class AdminFlowerPage : Page
    {
        private int? _flowerId;
        private byte[] _imageData;
        private bool _imageChanged;

        // Конструктор страницы
        public AdminFlowerPage(int? flowerId = null)
        {
            InitializeComponent();
            _flowerId = flowerId;
            LoadCategories();
            LoadData();
        }

        // Загрузка данных при редактировании
        private void LoadData()
        {
            if (_flowerId.HasValue)
            {
                TxtTitle.Text = "Редактирование товара";

                using var context = new FlowerShopDbContext();
                var flower = context.Flowers.Find(_flowerId.Value);

                if (flower != null)
                {
                    TBoxName.Text = flower.Name;
                    TBoxDescription.Text = flower.Description;
                    TBoxPrice.Text = flower.Price.ToString("F2");
                    TBoxStock.Text = flower.Stockquantity.ToString();

                    if (flower.Categoryid.HasValue)
                    {
                        ComboBoxCategories.SelectedValue = flower.Categoryid.Value;
                    }

                    if (flower.ImageData != null && flower.ImageData.Length > 0)
                    {
                        _imageData = flower.ImageData;
                        ShowImagePreview(_imageData);
                        TxtImageName.Text = "Текущее изображение";
                    }
                }
            }
            else
            {
                TxtTitle.Text = "Добавление товара";
            }

            TBoxName.Focus();
        }

        // Загрузка категорий
        private void LoadCategories()
        {
            using var context = new FlowerShopDbContext();
            var categories = context.Categories.ToList();
            ComboBoxCategories.ItemsSource = categories;
            ComboBoxCategories.DisplayMemberPath = "Name";
            ComboBoxCategories.SelectedValuePath = "Id";

            if (categories.Any())
            {
                ComboBoxCategories.SelectedIndex = 0;
            }
        }

        // Показ изображения в превью
        private void ShowImagePreview(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                ImagePreview.Source = new BitmapImage(new Uri("pack://application:,,,/Images/no_photo.png"));
                return;
            }

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(imageData);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            ImagePreview.Source = bitmap;
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
                    _imageData = File.ReadAllBytes(openFileDialog.FileName);
                    _imageChanged = true;

                    ShowImagePreview(_imageData);
                    TxtImageName.Text = Path.GetFileName(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ShowError($"Ошибка загрузки изображения: {ex.Message}");
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
            // Валидация
            if (string.IsNullOrWhiteSpace(TBoxName.Text))
            {
                ShowError("Введите название товара");
                TBoxName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TBoxDescription.Text))
            {
                ShowError("Введите описание товара");
                return;
            }

            if (!decimal.TryParse(TBoxPrice.Text, out decimal price) || price <= 0)
            {
                ShowError("Введите корректную цену");
                return;
            }

            if (!int.TryParse(TBoxStock.Text, out int stock) || stock < 0)
            {
                ShowError("Введите корректное количество");
                return;
            }

            if (ComboBoxCategories.SelectedItem == null)
            {
                ShowError("Выберите категорию");
                return;
            }

            try
            {
                using var context = new FlowerShopDbContext();

                if (_flowerId.HasValue)
                {
                    // Редактирование
                    var flower = context.Flowers.Find(_flowerId.Value);
                    if (flower != null)
                    {
                        flower.Name = TBoxName.Text.Trim();
                        flower.Description = TBoxDescription.Text.Trim();
                        flower.Price = price;
                        flower.Stockquantity = stock;
                        flower.Categoryid = ((Category)ComboBoxCategories.SelectedItem).Id;

                        if (_imageChanged && _imageData != null)
                        {
                            flower.ImageData = _imageData;
                        }

                        context.SaveChanges();
                    }
                }
                else
                {
                    // Добавление
                    var flower = new Flower
                    {
                        Name = TBoxName.Text.Trim(),
                        Description = TBoxDescription.Text.Trim(),
                        Price = price,
                        Stockquantity = stock,
                        Categoryid = ((Category)ComboBoxCategories.SelectedItem).Id,
                        ImageData = _imageData
                    };

                    context.Flowers.Add(flower);
                    context.SaveChanges();
                }

                MessageBox.Show(
                    _flowerId.HasValue ? "Товар обновлён" : "Товар добавлен",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                NavigationService.Navigate(new AdminFlowersPage());
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка сохранения: {ex.Message}");
            }
        }

        // Отмена и возврат к списку
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminFlowersPage());
        }

        // Навигация по меню
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AboutPage());
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new CartPage());
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ProfilePage());

        // Показать ошибку
        private void ShowError(string message)
        {
            TxtError.Text = message;
            TxtError.Visibility = Visibility.Visible;
            TxtSuccess.Visibility = Visibility.Collapsed;
        }

        private void ComboBoxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
    }
}