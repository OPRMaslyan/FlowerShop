using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlowerShop.Pages.AdminPanel
{
    public partial class AdminOrdersPage : Page
    {
        private List<OrderDisplay> _orders;

        // Конструктор страницы
        public AdminOrdersPage()
        {
            InitializeComponent();
            LoadOrders();
        }

        // Загрузка заказов из базы данных
        private void LoadOrders()
        {
            using var context = new FlowerShopDbContext();
            var ordersFromDb = context.Orders.Include(o => o.User).ToList();

            _orders = new List<OrderDisplay>();
            foreach (var order in ordersFromDb)
            {
                var items = context.Orderitems
                    .Where(oi => oi.Orderid == order.Id)
                    .Include(oi => oi.Flower)
                    .ToList();

                var itemsList = string.Join(", ", items.Select(i =>
                    $"{i.Flower?.Name ?? "Удалён"} (×{i.Quantity})"));

                _orders.Add(new OrderDisplay
                {
                    Id = order.Id,
                    OrderDate = order.Orderdate ?? DateTime.Now,
                    CustomerName = order.User?.Username ?? "Неизвестно",
                    CustomerPhone = order.User?.Email ?? "—",
                    TotalAmount = order.Totalamount,
                    Status = order.Status ?? "Pending",
                    ItemsList = itemsList,
                    ItemsCount = items.Count
                });
            }

            _orders = _orders.OrderByDescending(o => o.OrderDate).ToList();
            ItemsControlOrders.ItemsSource = _orders;
            TxtNoOrders.Visibility = _orders.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        // Установка выбранного статуса при загрузке ComboBox
        private void ComboBoxStatus_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox &&
                comboBox.DataContext is OrderDisplay order)
            {
                foreach (var item in comboBox.Items)
                {
                    if (item is ComboBoxItem comboBoxItem &&
                        comboBoxItem.Tag?.ToString() == order.Status)
                    {
                        comboBox.SelectedItem = comboBoxItem;
                        break;
                    }
                }
            }
        }

        // Обработка смены статуса заказа
        private void ComboBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox &&
                comboBox.Tag is int orderId &&
                comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                var newStatus = selectedItem.Tag?.ToString();
                if (!string.IsNullOrEmpty(newStatus))
                {
                    try
                    {
                        using var context = new FlowerShopDbContext();
                        var order = context.Orders.FirstOrDefault(o => o.Id == orderId);

                        if (order != null)
                        {
                            order.Status = newStatus;
                            context.SaveChanges();

                            var orderDisplay = _orders.FirstOrDefault(o => o.Id == orderId);
                            if (orderDisplay != null)
                            {
                                orderDisplay.Status = newStatus;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        LoadOrders();
                    }
                }
            }
        }

        // Переход к деталям заказа
        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int orderId)
            {
                NavigationService.Navigate(new OrderDetailsPage(orderId));
            }
        }

        // Удаление заказа с каскадным удалением связанных записей
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int orderId)
            {
                if (MessageBox.Show($"Удалить заказ номер {orderId}?", "Подтверждение",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using var context = new FlowerShopDbContext();
                        var order = context.Orders.FirstOrDefault(o => o.Id == orderId);
                        if (order != null)
                        {
                            // Удаляем позиции заказа
                            var orderItems = context.Orderitems.Where(oi => oi.Orderid == orderId).ToList();
                            context.Orderitems.RemoveRange(orderItems);

                            // Удаляем доставку если есть
                            var delivery = context.Deliveries.FirstOrDefault(d => d.Orderid == orderId);
                            if (delivery != null)
                            {
                                context.Deliveries.Remove(delivery);
                            }

                            // Удаляем заказ
                            context.Orders.Remove(order);
                            context.SaveChanges();

                            MessageBox.Show("Заказ удалён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadOrders();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // Навигация на главную админ-панели
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminPanelPage());
        }

        // Навигация по меню
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AboutPage());
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new CartPage());
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ProfilePage());
    }

    // Класс для отображения заказа в списке
    public class OrderDisplay
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ItemsList { get; set; }
        public int ItemsCount { get; set; }
    }
}