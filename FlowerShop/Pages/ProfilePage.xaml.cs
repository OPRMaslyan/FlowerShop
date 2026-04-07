using FlowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlowerShop.Pages
{
    public partial class ProfilePage : Page
    {
        private bool _isEditMode;
        private User _currentUser;

        // Конструктор страницы
        public ProfilePage()
        {
            InitializeComponent();
            LoadProfile();
            LoadOrders();
        }

        // Загрузка данных профиля
        private void LoadProfile()
        {
            _currentUser = App.CurrentUser;
            if (_currentUser == null)
            {
                NavigationService.Navigate(new AuthPage());
                return;
            }

            TBoxUsername.Text = _currentUser.Username;
            TBoxEmail.Text = _currentUser.Email;
            TBoxRole.Text = _currentUser.Role == "Admin" ? "Администратор" : "Покупатель";

            TBoxCreatedAt.Text = _currentUser.Createdat.HasValue
                ? _currentUser.Createdat.Value.ToString("dd.MM.yyyy HH:mm")
                : "—";
        }

        // Загрузка заказов пользователя
        private void LoadOrders()
        {
            using var context = new FlowerShopDbContext();

            var ordersFromDb = context.Orders
                .Where(o => o.Userid == _currentUser.Id)
                .ToList();

            var orders = new List<OrderDisplayItem>();
            foreach (var order in ordersFromDb)
            {
                var itemsCount = context.Orderitems.Count(oi => oi.Orderid == order.Id);

                string statusText = order.Status switch
                {
                    "Pending" => "Ожидает оплаты",
                    "Paid" => "Оплачен",
                    "Processing" => "В обработке",
                    "Shipped" => "Отправлен",
                    "Delivered" => "Доставлен",
                    "Cancelled" => "Отменён",
                    _ => order.Status
                };

                Brush statusColor = order.Status switch
                {
                    "Pending" => Brushes.Orange,
                    "Paid" => Brushes.Blue,
                    "Processing" => Brushes.Orange,
                    "Shipped" => Brushes.Purple,
                    "Delivered" => Brushes.Green,
                    "Cancelled" => Brushes.Red,
                    _ => Brushes.Gray
                };

                orders.Add(new OrderDisplayItem
                {
                    Id = order.Id,
                    OrderDate = order.Orderdate.HasValue ? order.Orderdate.Value : DateTime.Now,
                    TotalAmount = order.Totalamount,
                    Status = statusText,
                    StatusColor = statusColor,
                    ItemsCount = itemsCount
                });
            }

            orders = orders.OrderByDescending(o => o.OrderDate).ToList();
            ItemsControlOrders.ItemsSource = orders;
            TxtNoOrders.Visibility = orders.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        // Включение режима редактирования
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            _isEditMode = true;
            TBoxEmail.IsEnabled = true;
            TBoxUsername.IsEnabled = true;
            BtnEdit.Visibility = Visibility.Collapsed;
            BtnSave.Visibility = Visibility.Visible;
            BtnCancel.Visibility = Visibility.Visible;
            TBoxUsername.Focus();
        }

        // Сохранение изменений профиля
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var email = TBoxEmail.Text.Trim();
            var username = TBoxUsername.Text.Trim();

            if (string.IsNullOrEmpty(username)) { ShowMessage("Введите логин", true); TBoxUsername.Focus(); return; }
            if (username.Length < 3 || username.Length > 100) { ShowMessage("Логин от 3 до 100 символов", true); return; }
            if (string.IsNullOrEmpty(email)) { ShowMessage("Введите email", true); TBoxEmail.Focus(); return; }
            if (!IsValidEmail(email)) { ShowMessage("Некорректный email", true); return; }

            try
            {
                using var context = new FlowerShopDbContext();
                var user = context.Users.Find(_currentUser.Id);
                if (user != null)
                {
                    user.Username = username;
                    user.Email = email;
                    context.SaveChanges();
                    _currentUser.Username = username;
                    _currentUser.Email = email;
                    App.CurrentUser = _currentUser;
                }
                _isEditMode = false;
                TBoxEmail.IsEnabled = false;
                TBoxUsername.IsEnabled = false;
                BtnEdit.Visibility = Visibility.Visible;
                BtnSave.Visibility = Visibility.Collapsed;
                BtnCancel.Visibility = Visibility.Collapsed;
                ShowMessage("Данные обновлены", false);
            }
            catch (Exception ex) { ShowMessage($"Ошибка: {ex.Message}", true); }
        }

        // Отмена редактирования
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _isEditMode = false;
            TBoxEmail.IsEnabled = false;
            TBoxUsername.IsEnabled = false;
            TBoxEmail.Text = _currentUser.Email;
            TBoxUsername.Text = _currentUser.Username;
            BtnEdit.Visibility = Visibility.Visible;
            BtnSave.Visibility = Visibility.Collapsed;
            BtnCancel.Visibility = Visibility.Collapsed;
        }

        // Удаление профиля с каскадным удалением связанных записей
        private void BtnDeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить профиль?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using var context = new FlowerShopDbContext();
                    var user = context.Users.Find(_currentUser.Id);
                    if (user != null)
                    {
                        // Удаляем отзывы
                        var reviews = context.Reviews.Where(r => r.Userid == _currentUser.Id).ToList();
                        context.Reviews.RemoveRange(reviews);

                        // Удаляем корзину
                        var cartItems = context.Cartitems.Where(c => c.Userid == _currentUser.Id).ToList();
                        context.Cartitems.RemoveRange(cartItems);

                        // Удаляем заказы и их позиции
                        var orders = context.Orders.Where(o => o.Userid == _currentUser.Id).ToList();
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

                        App.CurrentUser = null;
                        MessageBox.Show("Профиль удалён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        NavigationService.Navigate(new AuthPage());
                    }
                }
                catch (Exception ex) { ShowMessage($"Ошибка: {ex.Message}", true); }
            }
        }

        // Выход из аккаунта
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            NavigationService.Navigate(new AuthPage());
        }

        // Переход к деталям заказа
        private void BtnOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int orderId)
            {
                NavigationService.Navigate(new OrderDetailsPage(orderId));
            }
        }

        // Навигация по меню
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AboutPage());
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new CartPage());
        private void BtnProfile_Click(object sender, RoutedEventArgs e) { }

        // Показ сообщения пользователю
        private void ShowMessage(string message, bool isError)
        {
            TxtMessage.Text = message;
            TxtMessage.Foreground = isError ? Brushes.Red : Brushes.Green;
            TxtMessage.Visibility = Visibility.Visible;
        }

        // Проверка корректности email
        private bool IsValidEmail(string email)
        {
            try { return new MailAddress(email).Address == email; }
            catch { return false; }
        }
    }

    // Класс для отображения заказа в профиле
    public class OrderDisplayItem
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public Brush StatusColor { get; set; }
        public int ItemsCount { get; set; }
    }
}