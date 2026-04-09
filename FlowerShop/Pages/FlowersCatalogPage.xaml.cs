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

        private void LoadData()
        {
            using (var context = new FlowerShopDbContext())
            {
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

                _allCategories = context.Categories.ToList();
                _allCategories.Insert(0, new Category { Id = 0, Name = "Все категории" });
                ComboBoxSort.SelectedIndex = 0;
                ComboBoxCategories.ItemsSource = _allCategories;
                ComboBoxCategories.SelectedValuePath = "Id";
                ComboBoxCategories.SelectedIndex = 0;
                DisplayFlowers(_allFlowers);
            }
        }

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

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ComboBoxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = _allFlowers.AsEnumerable();

            var searchText = TBoxSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(f => f.Name.ToLower().Contains(searchText));
            }

            if (ComboBoxCategories.SelectedItem is Category selectedCategory && selectedCategory.Id > 0)
            {
                filtered = filtered.Where(f => f.CategoryId == selectedCategory.Id);
            }

            if (ComboBoxSort.SelectedItem is ComboBoxItem sortItem)
            {
                var sortTag = sortItem.Tag?.ToString();
                filtered = sortTag switch
                {
                    "price_asc" => filtered.OrderBy(f => f.Price),
                    "price_desc" => filtered.OrderByDescending(f => f.Price),
                    "name_asc" => filtered.OrderBy(f => f.Name),
                    "name_desc" => filtered.OrderByDescending(f => f.Name),
                    _ => filtered.OrderBy(f => f.Id)
                };
            }

            DisplayFlowers(filtered.ToList());
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            TBoxSearch.Clear();
            ComboBoxCategories.SelectedIndex = 0;
            ComboBoxSort.SelectedIndex = 0;
            DisplayFlowers(_allFlowers);
        }

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("Войдите в аккаунт", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (sender is Button btn && btn.Tag is Flower flower)
            {
                using var context = new FlowerShopDbContext();
                var existingItem = context.Cartitems
                    .FirstOrDefault(c => c.Userid == App.CurrentUser.Id && c.Flowerid == flower.Id);

                if (existingItem != null)
                {
                    if (existingItem.Quantity < flower.Stockquantity)
                    {
                        existingItem.Quantity++;
                        context.SaveChanges();
                        MessageBox.Show("Количество увеличено", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Недостаточно товара на складе", "Внимание",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    var cartItem = new Cartitem
                    {
                        Userid = App.CurrentUser.Id,
                        Flowerid = flower.Id,
                        Quantity = 1
                    };
                    context.Cartitems.Add(cartItem);
                    context.SaveChanges();
                    MessageBox.Show("Товар добавлен в корзину", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AboutPage());
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new CartPage());
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ProfilePage());
    }

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