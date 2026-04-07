using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlowerShop.Pages.AdminPanel
{
    public partial class AdminClientsPage : Page
    {
        private List<ClientDisplayItem> _allClients;

        // Конструктор страницы
        public AdminClientsPage()
        {
            InitializeComponent();
            LoadClients();
        }

        // Загрузка клиентов из базы данных
        private void LoadClients()
        {
            using var context = new FlowerShopDbContext();

            var users = context.Users
                .Include(u => u.Role)
                .Include(u => u.Orders)
                .Where(u => u.Role != null && (u.Role.Name == "Customer" || u.Role.Name == "client"))
                .ToList();

            _allClients = users.Select(u => new ClientDisplayItem
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                RegistrationDate = u.Createdat ?? DateTime.MinValue,
                OrdersCount = u.Orders.Count
            })
            .OrderByDescending(u => u.RegistrationDate)
            .ToList();

            ApplyFilters();
        }

        // Применение фильтра поиска
        private void ApplyFilters()
        {
            var searchText = TBoxSearch?.Text?.Trim().ToLower() ?? "";

            var filtered = string.IsNullOrEmpty(searchText)
                ? _allClients
                : _allClients.Where(c => c.Username.ToLower().Contains(searchText)).ToList();

            DataGridClients.ItemsSource = filtered;
            TxtNoClients.Visibility = filtered.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        // Обработчик изменения текста поиска
        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        // Сброс поиска
        private void BtnResetSearch_Click(object sender, RoutedEventArgs e)
        {
            TBoxSearch.Clear();
            ApplyFilters();
            TBoxSearch.Focus();
        }

        // Удаление клиента с каскадным удалением связанных записей
        private void BtnDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int clientId)
            {
                var client = _allClients.FirstOrDefault(c => c.Id == clientId);
                if (client == null) return;

                var result = MessageBox.Show(
                    $"Удалить клиента {client.Username}?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using var context = new FlowerShopDbContext();
                        var user = context.Users.Find(clientId);
                        if (user != null)
                        {
                            // Удаляем отзывы пользователя
                            var reviews = context.Reviews.Where(r => r.Userid == clientId).ToList();
                            context.Reviews.RemoveRange(reviews);

                            // Удаляем элементы корзины
                            var cartItems = context.Cartitems.Where(c => c.Userid == clientId).ToList();
                            context.Cartitems.RemoveRange(cartItems);

                            // Удаляем заказы пользователя (включая позиции заказов)
                            var orders = context.Orders.Where(o => o.Userid == clientId).ToList();
                            foreach (var order in orders)
                            {
                                var orderItems = context.Orderitems.Where(oi => oi.Orderid == order.Id).ToList();
                                context.Orderitems.RemoveRange(orderItems);

                                var delivery = context.Deliveries.FirstOrDefault(d => d.Orderid == order.Id);
                                if (delivery != null)
                                {
                                    context.Deliveries.Remove(delivery);
                                }
                            }
                            context.Orders.RemoveRange(orders);

                            // Удаляем пользователя
                            context.Users.Remove(user);
                            context.SaveChanges();

                            LoadClients();
                            MessageBox.Show("Клиент удалён", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
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

    // Класс для отображения клиента в списке
    public class ClientDisplayItem
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int OrdersCount { get; set; }
    }
}