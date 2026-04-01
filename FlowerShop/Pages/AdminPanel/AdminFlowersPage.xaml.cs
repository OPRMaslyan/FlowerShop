using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FlowerShop.Pages.AdminPanel
{
    public partial class AdminFlowersPage : Page
    {
        private List<AdminFlowerItem> _allFlowers;

        // Конструктор страницы
        public AdminFlowersPage()
        {
            InitializeComponent();
            LoadFlowers();
        }

        // Загрузка всех товаров из базы данных
        private void LoadFlowers()
        {
            using var context = new FlowerShopDbContext();
            var flowers = context.Flowers
                 .Include(f => f.Category)
                 .ToList();

            _allFlowers = new List<AdminFlowerItem>();
            foreach (var flower in flowers)
            {
                _allFlowers.Add(new AdminFlowerItem
                {
                    Id = flower.Id,
                    Name = flower.Name,
                    Price = flower.Price,
                    Stockquantity = flower.Stockquantity,
                    CategoryName = flower.Category?.Name ?? "Без категории",
                    DisplayImage = ConvertImage(flower.ImageData),
                    Flower = flower
                });
            }

            ComboBoxSort.SelectedIndex = 0;
            ApplyFilters();
        }

        // Конвертация byte[] в BitmapImage
        private BitmapImage ConvertImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return new BitmapImage(new Uri("pack://application:,,,/Images/no_photo.png"));
            }

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(imageData);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        // Применение поиска и сортировки
        private void ApplyFilters()
        {
            var filtered = _allFlowers.AsEnumerable();

            var searchText = TBoxSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(f => f.Name.ToLower().Contains(searchText));
            }

            if (ComboBoxSort.SelectedItem is ComboBoxItem sortItem)
            {
                var sortTag = sortItem.Tag?.ToString();
                filtered = sortTag switch
                {
                    "name_asc" => filtered.OrderBy(f => f.Name),
                    "name_desc" => filtered.OrderByDescending(f => f.Name),
                    "price_asc" => filtered.OrderBy(f => f.Price),
                    "price_desc" => filtered.OrderByDescending(f => f.Price),
                    "stock_desc" => filtered.OrderByDescending(f => f.Stockquantity),
                    _ => filtered.OrderBy(f => f.Id)
                };
            }

            var resultList = filtered.ToList();
            ItemsControlFlowers.ItemsSource = resultList;
            TxtNoResults.Visibility = resultList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        // Обработчик изменения текста поиска
        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        // Обработчик изменения сортировки
        private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        // Сброс фильтров
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            TBoxSearch.Clear();
            ComboBoxSort.SelectedIndex = 0;
            ApplyFilters();
        }

        // Переход к добавлению товара
        private void BtnAddFlower_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminFlowerPage());
        }

        // Переход к редактированию товара
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int flowerId)
            {
                NavigationService.Navigate(new AdminFlowerPage(flowerId));
            }
        }

        // Удаление товара с каскадным удалением связанных записей
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int flowerId)
            {
                var result = MessageBox.Show(
                    "Вы уверены, что хотите удалить этот товар?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using var context = new FlowerShopDbContext();
                        var flower = context.Flowers.Find(flowerId);
                        if (flower != null)
                        {
                            // Удаляем из корзины всех пользователей
                            var cartItems = context.Cartitems.Where(c => c.Flowerid == flowerId).ToList();
                            context.Cartitems.RemoveRange(cartItems);

                            // Удаляем из позиций заказов (в заказах останется запись о удалённом товаре)
                            var orderItems = context.Orderitems.Where(oi => oi.Flowerid == flowerId).ToList();
                            context.Orderitems.RemoveRange(orderItems);

                            // Удаляем отзывы о товаре
                            var reviews = context.Reviews.Where(r => r.Flowerid == flowerId).ToList();
                            context.Reviews.RemoveRange(reviews);

                            // Удаляем товар
                            context.Flowers.Remove(flower);
                            context.SaveChanges();

                            _allFlowers.RemoveAll(f => f.Id == flowerId);
                            ApplyFilters();

                            MessageBox.Show("Товар успешно удалён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // Навигация по меню
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AboutPage());
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new CartPage());
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ProfilePage());
    }

    // Класс для отображения товара в админ-панели
    public class AdminFlowerItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stockquantity { get; set; }
        public string CategoryName { get; set; }
        public BitmapImage DisplayImage { get; set; }
        public Flower Flower { get; set; }
    }
}