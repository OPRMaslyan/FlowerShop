using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlowerShop.Pages
{
    public partial class OrderDetailsPage : Page
    {
        private int _orderId;

        public OrderDetailsPage(int orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            LoadOrderDetails();
        }

        private void LoadOrderDetails()
        {
            using var context = new FlowerShopDbContext();
            var order = context.Orders.FirstOrDefault(o => o.Id == _orderId);

            if (order == null)
            {
                MessageBox.Show("Заказ не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService.Navigate(new ProfilePage());
                return;
            }

            // Заголовок
            TxtOrderTitle.Text = $"📦 Заказ №{order.Id}";

            // 👇 Исправлено для DateTime?
            TxtOrderDate.Text = order.Orderdate.HasValue
                ? order.Orderdate.Value.ToString("dd.MM.yyyy HH:mm")
                : "—";

            // Статус
            var (statusText, statusColor) = GetStatus(order.Status);
            TxtStatus.Text = statusText;
            BadgeStatus.Background = statusColor;

            // 👇 Шаг 1: Загружаем товары из БД
            var itemsFromDb = context.Orderitems
                .Where(oi => oi.Orderid == _orderId)
                .Include(oi => oi.Flower)
                .ToList();  // 👈 Материализуем запрос

            // 👇 Шаг 2: Обрабатываем в памяти (null-conditional работает)
            var items = new List<OrderItemDisplay>();
            foreach (var item in itemsFromDb)
            {
                items.Add(new OrderItemDisplay
                {
                    // 👇 Исправлено: обработка в памяти, не в LINQ
                    FlowerName = item.Flower != null ? item.Flower.Name : "Удалён",
                    Price = item.Unitprice,
                    Quantity = item.Quantity,
                    Subtotal = item.Unitprice * item.Quantity
                });
            }

            ItemsControlItems.ItemsSource = items;
            TxtTotal.Text = $"{order.Totalamount:F2} ₽";
        }

        private (string, Brush) GetStatus(string status)
        {
            return status switch
            {
                "Pending" => ("В обработке", Brushes.Orange),
                "Completed" => ("Выполнен", Brushes.Green),
                "Cancelled" => ("Отменён", Brushes.Red),
                _ => (status, Brushes.Gray)
            };
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ProfilePage());
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AboutPage());
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new CartPage());
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ProfilePage());
    }

    public class OrderItemDisplay
    {
        public string FlowerName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}