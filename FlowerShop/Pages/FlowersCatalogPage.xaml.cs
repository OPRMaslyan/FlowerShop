using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FlowerShop.Pages
{
    public partial class FlowersCatalogPage : Page
    {
        public FlowersCatalogPage()
        {
            InitializeComponent();
            LoadFlowers();
        }

        private void LoadFlowers()
        {
            using (var context = new FlowerShopDbContext())
            {
                var flowers = context.Flowers
                     .Include(f => f.Category)
                     .ToList();

                // Создаём список с уже конвертированными изображениями
                var flowerItems = new List<FlowerDisplayItem>();
                foreach (var flower in flowers)
                {
                    flowerItems.Add(new FlowerDisplayItem
                    {
                        Flower = flower,
                        Name = flower.Name,
                        Price = flower.Price,
                        Category = flower.Category,
                        DisplayImage = ConvertImage(flower.ImageData)
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
                // Заглушка, если нет картинки
                return new BitmapImage(new Uri("/Images/no_photo.png", UriKind.Relative));
            }

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(imageData);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Как нибудь потом", "Будущее", MessageBoxButton.OKCancel);
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

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }
    }

    // Класс-обёртка для отображения (прямо в этом же файле!)
    public class FlowerDisplayItem
    {
        public Flower Flower { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Category Category { get; set; }
        public BitmapImage DisplayImage { get; set; }
    }
}