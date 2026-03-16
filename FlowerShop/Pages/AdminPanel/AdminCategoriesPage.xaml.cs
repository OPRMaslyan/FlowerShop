using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlowerShop.Pages.AdminPanel
{
    public partial class AdminCategoriesPage : Page
    {
        private List<CategoryDisplayItem> _allCategories;

        public AdminCategoriesPage()
        {
            InitializeComponent();
            LoadCategories();
        }

        // Загрузка категорий из БД
        private void LoadCategories()
        {
            using var context = new FlowerShopDbContext();

            _allCategories = context.Categories
                .Include(c => c.Flowers)
                .Select(c => new CategoryDisplayItem
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description ?? "—",
                    FlowersCount = c.Flowers.Count
                })
                .OrderBy(c => c.Name)
                .ToList();

            DataGridCategories.ItemsSource = _allCategories;
            TxtNoCategories.Visibility = _allCategories.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        // Добавить категорию
        private void BtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCategoryPage());
        }

        // Редактировать категорию
        private void BtnEditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int categoryId)
            {
                NavigationService.Navigate(new AdminCategoryPage(categoryId));
            }
        }

        // Удалить категорию
        private void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int categoryId)
            {
                var category = _allCategories.FirstOrDefault(c => c.Id == categoryId);
                if (category == null) return;

                // Проверка: есть ли товары в категории
                if (category.FlowersCount > 0)
                {
                    MessageBox.Show(
                        $"Невозможно удалить категорию \"{category.Name}\": в ней есть товары ({category.FlowersCount} шт.)",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Подтверждение удаления
                var result = MessageBox.Show(
                    $"Удалить категорию \"{category.Name}\"?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using var context = new FlowerShopDbContext();
                        var cat = context.Categories.Find(categoryId);
                        if (cat != null)
                        {
                            context.Categories.Remove(cat);
                            context.SaveChanges();

                            _allCategories.RemoveAll(c => c.Id == categoryId);
                            DataGridCategories.ItemsSource = null;
                            DataGridCategories.ItemsSource = _allCategories;

                            TxtNoCategories.Visibility = _allCategories.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                            MessageBox.Show("Категория удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // Навигация
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AboutPage());
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Корзина в разработке", "Информация");
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ProfilePage());
    }

    // Вспомогательный класс для отображения категории
    public class CategoryDisplayItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int FlowersCount { get; set; }
    }
}