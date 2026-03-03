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
        public AdminFlowersPage()
        {
            InitializeComponent();
            LoadFlowers();
        }

        // Загрузка всех товаров
        private void LoadFlowers()
        {
            using (var context = new FlowerShopDbContext())
            {
                var flowers = context.Flowers
                     .Include(f => f.Category)
                     .ToList();

                var flowerItems = new List<AdminFlowerItem>();
                foreach (var flower in flowers)
                {
                    flowerItems.Add(new AdminFlowerItem
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

                ItemsControlFlowers.ItemsSource = flowerItems;
            }
        }

        // Конвертация byte[] → BitmapImage
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

        // Добавить товар
        private void BtnAddFlower_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminAddFlowersPage());
        }

        // Редактировать товар
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Редактирование в разработке", "Информация");
            //if (sender is Button button && button.Tag is int flowerId)
            //{
            //    NavigationService.Navigate(new AdminEditFlowersPage());// Сюда поступает параметр flowerId
            //}
        }

        // Удалить товар
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
                        using (var context = new FlowerShopDbContext())
                        {
                            var flower = context.Flowers.Find(flowerId);
                            if (flower != null)
                            {
                                context.Flowers.Remove(flower);
                                context.SaveChanges();
                                MessageBox.Show("Товар успешно удалён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadFlowers(); // Обновить список
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // Навигация
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AboutPage());
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Корзина в разработке", "Информация");
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Профиль в разработке", "Информация");
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