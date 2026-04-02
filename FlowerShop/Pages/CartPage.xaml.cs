using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlowerShop.Pages
{
    public partial class CartPage : Page
    {
        private List<CartItemDisplay> _cartItems;

        // Конструктор страницы
        public CartPage()
        {
            InitializeComponent();
            LoadCart();
        }

        // Загрузка содержимого корзины
        private void LoadCart()
        {
            if (App.CurrentUser == null)
            {
                NavigationService.Navigate(new AuthPage());
                return;
            }

            using var context = new FlowerShopDbContext();
            _cartItems = context.Cartitems
                .Include(c => c.Flower)
                .Where(c => c.Userid == App.CurrentUser.Id)
                .Select(c => new CartItemDisplay
                {
                    Id = c.Id,
                    FlowerId = c.Flowerid ?? 0,
                    FlowerName = c.Flower != null ? c.Flower.Name : "Удалён",
                    Price = c.Flower != null ? c.Flower.Price : 0,
                    Quantity = c.Quantity,
                    Total = c.Flower != null ? c.Flower.Price * c.Quantity : 0
                })
                .ToList();

            ItemsControlCart.ItemsSource = _cartItems;
            TxtEmptyCart.Visibility = _cartItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            UpdateTotal();
        }

        // Обновление итоговой суммы
        private void UpdateTotal()
        {
            var total = _cartItems.Sum(i => i.Total);
            TxtTotal.Text = $"{total:F2} ₽";
        }

        // Увеличение количества товара
        private void BtnIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int cartItemId)
            {
                using var context = new FlowerShopDbContext();
                var item = context.Cartitems
                    .Include(c => c.Flower)
                    .FirstOrDefault(c => c.Id == cartItemId);

                if (item != null && item.Flower != null)
                {
                    if (item.Quantity < item.Flower.Stockquantity)
                    {
                        item.Quantity++;
                        context.SaveChanges();
                        LoadCart();
                    }
                    else
                    {
                        MessageBox.Show("Недостаточно товара на складе", "Внимание",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        // Уменьшение количества товара
        private void BtnDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int cartItemId)
            {
                using var context = new FlowerShopDbContext();
                var item = context.Cartitems.Find(cartItemId);
                if (item != null)
                {
                    if (item.Quantity > 1)
                    {
                        item.Quantity--;
                        context.SaveChanges();
                        LoadCart();
                    }
                    else
                    {
                        context.Cartitems.Remove(item);
                        context.SaveChanges();
                        LoadCart();
                    }
                }
            }
        }

        // Удаление товара из корзины
        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int cartItemId)
            {
                var result = MessageBox.Show("Удалить товар из корзины?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using var context = new FlowerShopDbContext();
                    var item = context.Cartitems.Find(cartItemId);
                    if (item != null)
                    {
                        context.Cartitems.Remove(item);
                        context.SaveChanges();
                        LoadCart();
                    }
                }
            }
        }

        // Оформление заказа
        private void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (_cartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Оформить заказ?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using var context = new FlowerShopDbContext();
                    var total = _cartItems.Sum(i => i.Total);

                    var order = new Order
                    {
                        Userid = App.CurrentUser.Id,
                        Orderdate = DateTime.Now,
                        Totalamount = total,
                        Status = "Pending"
                    };

                    context.Orders.Add(order);
                    context.SaveChanges();

                    foreach (var item in _cartItems)
                    {
                        var orderItem = new Orderitem
                        {
                            Orderid = order.Id,
                            Flowerid = item.FlowerId,
                            Quantity = item.Quantity,
                            Unitprice = item.Price
                        };
                        context.Orderitems.Add(orderItem);

                        var flower = context.Flowers.Find(item.FlowerId);
                        if (flower != null)
                        {
                            flower.Stockquantity -= item.Quantity;
                        }
                    }

                    var cartItems = context.Cartitems.Where(c => c.Userid == App.CurrentUser.Id).ToList();
                    context.Cartitems.RemoveRange(cartItems);
                    context.SaveChanges();

                    NavigationService.Navigate(new PaymentPage(order.Id, total));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Навигация по меню
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AboutPage());
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) { }
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ProfilePage());
    }

    // Класс для отображения товара в корзине
    public class CartItemDisplay
    {
        public int Id { get; set; }
        public int FlowerId { get; set; }
        public string FlowerName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}