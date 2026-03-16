using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FlowerShop.Pages
{
    public partial class FlowersCatalogPage : Page
    {
        private List<FlowerDisplayItem> _allFlowers;
        private List<Category> _allCategories;

        public FlowersCatalogPage()
        {
            InitializeComponent();
            LoadData();
        }

        // Загрузка данных
        private void LoadData()
        {
            using (var context = new FlowerShopDbContext())
            {
                // Загружаем все цветы
                var flowers = context.Flowers.Include(f => f.Category).ToList();
                _allFlowers = new List<FlowerDisplayItem>();
                foreach (var flower in flowers)
                {
                    _allFlowers.Add(new FlowerDisplayItem
                    {
                        Flower = flower,
                        Id = flower.Id,
                        Name = flower.Name,
                        Price = flower.Price,
                        Stockquantity = flower.Stockquantity,
                        CategoryName = flower.Category?.Name ?? "Без категории",
                        CategoryId = flower.Categoryid,
                        DisplayImage = ConvertImage(flower.ImageData)
                    });
                }

                // Загружаем категории для фильтра
                _allCategories = context.Categories.ToList();

                // Добавляем пункт "Все категории" В НАЧАЛО списка
                _allCategories.Insert(0, new Category { Id = 0, Name = "Все категории" });
                ComboBoxSort.SelectedIndex = 0;

                // Устанавливаем ItemsSource
                ComboBoxCategories.ItemsSource = _allCategories;
                ComboBoxCategories.SelectedValuePath = "Id";

                // Выбираем первый элемент
                ComboBoxCategories.SelectedIndex = 0;

                // Отображаем все товары
                DisplayFlowers(_allFlowers);
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

        // Отображение товаров
        private void DisplayFlowers(List<FlowerDisplayItem> flowers)
        {
            if (flowers.Count == 0)
            {
                ItemsControlFlowers.ItemsSource = null;
                TxtNoResults.Visibility = Visibility.Visible;
            }
            else
            {
                ItemsControlFlowers.ItemsSource = flowers;
                TxtNoResults.Visibility = Visibility.Collapsed;
            }
        }

        // Поиск по названию
        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        // Фильтр по категории
        private void ComboBoxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        // Сортировка
        private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        // Применение всех фильтров
        private void ApplyFilters()
        {
            var filtered = _allFlowers.AsEnumerable();

            // Поиск
            var searchText = TBoxSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(f => f.Name.ToLower().Contains(searchText));
            }

            // Категория
            if (ComboBoxCategories.SelectedItem is Category selectedCategory && selectedCategory.Id > 0)
            {
                filtered = filtered.Where(f => f.CategoryId == selectedCategory.Id);
            }

            // Сортировка
            if (ComboBoxSort.SelectedItem is ComboBoxItem sortItem)
            {
                var sortTag = sortItem.Tag?.ToString();
                switch (sortTag)
                {
                    case "price_asc":
                        filtered = filtered.OrderBy(f => f.Price);
                        break;
                    case "price_desc":
                        filtered = filtered.OrderByDescending(f => f.Price);
                        break;
                    case "name_asc":
                        filtered = filtered.OrderBy(f => f.Name);
                        break;
                    case "name_desc":
                        filtered = filtered.OrderByDescending(f => f.Name);
                        break;
                    default:
                        filtered = filtered.OrderBy(f => f.Id);
                        break;
                }
            }

            DisplayFlowers(filtered.ToList());
        }

        // Сброс фильтров
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            TBoxSearch.Clear();
            ComboBoxCategories.SelectedIndex = 0;
            ComboBoxSort.SelectedIndex = 0;
            DisplayFlowers(_allFlowers);
        }

        // Навигация
        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Товар добавлен в корзину", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AboutPage());
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Корзина в разработке", "Информация");
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ProfilePage());
    }

    // Класс для отображения товара
    public class FlowerDisplayItem
    {
        public int Id { get; set; }
        public Flower Flower { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stockquantity { get; set; }
        public string CategoryName { get; set; }
        public int? CategoryId { get; set; }
        public BitmapImage DisplayImage { get; set; }
    }
}