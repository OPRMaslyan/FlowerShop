using FlowerShop.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlowerShop.Pages.AdminPanel
{
    public partial class AdminCategoryPage : Page
    {
        private int? _categoryId;

        public AdminCategoryPage(int? categoryId = null)
        {
            InitializeComponent();
            _categoryId = categoryId;
            LoadData();
        }

        // Загрузка данных при редактировании
        private void LoadData()
        {
            if (_categoryId.HasValue)
            {
                TxtTitle.Text = "Редактирование категории";

                using var context = new FlowerShopDbContext();
                var category = context.Categories.Find(_categoryId.Value);

                if (category != null)
                {
                    TBoxName.Text = category.Name;
                    TBoxDescription.Text = category.Description;
                }
            }

            TBoxName.Focus();
        }

        // Сохранение
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var name = TBoxName.Text.Trim();
            var description = TBoxDescription.Text.Trim();

            // Валидация
            if (string.IsNullOrEmpty(name))
            {
                ShowError("Название категории обязательно");
                TBoxName.Focus();
                return;
            }

            if (name.Length > 100)
            {
                ShowError("Название не должно превышать 100 символов");
                return;
            }

            try
            {
                using var context = new FlowerShopDbContext();

                if (_categoryId.HasValue)
                {
                    // Редактирование
                    var category = context.Categories.Find(_categoryId.Value);
                    if (category != null)
                    {
                        category.Name = name;
                        category.Description = string.IsNullOrEmpty(description) ? null : description;
                        context.SaveChanges();
                    }
                }
                else
                {
                    // Добавление
                    var newCategory = new Category
                    {
                        Name = name,
                        Description = string.IsNullOrEmpty(description) ? null : description
                    };
                    context.Categories.Add(newCategory);
                    context.SaveChanges();
                }

                MessageBox.Show(
                    _categoryId.HasValue ? "Категория обновлена" : "Категория добавлена",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                NavigationService.Navigate(new AdminCategoriesPage());
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка: {ex.Message}");
            }
        }

        // Отмена
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCategoriesPage());
        }

        // Показать ошибку
        private void ShowError(string message)
        {
            TxtError.Text = message;
            TxtError.Visibility = Visibility.Visible;
        }

        // Навигация
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AboutPage());
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Корзина в разработке", "Информация");
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Профиль в разработке", "Информация");
    }
}