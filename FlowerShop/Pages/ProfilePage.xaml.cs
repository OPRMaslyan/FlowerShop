using BCrypt.Net;
using FlowerShop.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;  

namespace FlowerShop.Pages
{
    public partial class ProfilePage : Page
    {
        private bool _isEditMode;
        private User _currentUser;

        public ProfilePage()
        {
            InitializeComponent();
            LoadProfile();
            LoadOrders();
        }

        //Загрузка данных профиля
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
            TBoxCreatedAt.Text = _currentUser.Createdat?.ToString("dd.MM.yyyy HH:mm") ?? "—";
        }

        //Загрузка заказов
        private void LoadOrders()
        {
            using var context = new FlowerShopDbContext();
            var orders = context.Orders
                .Where(o => o.Userid == _currentUser.Id)
                .Select(o => new OrderDisplayItem
                {
                    Id = o.Id,
                    OrderDate = o.Orderdate ?? DateTime.Now,
                    TotalAmount = o.Totalamount,
                    Status = o.Status == "Pending" ? "В обработке" :
                             o.Status == "Completed" ? "Выполнен" :
                             o.Status == "Cancelled" ? "Отменён" : o.Status,
                    ItemsCount = context.Orderitems.Count(oi => oi.Orderid == o.Id)
                })
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            DataGridOrders.ItemsSource = orders;
            TxtNoOrders.Visibility = orders.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        //Редактирование
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

        //Сохранение
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var email = TBoxEmail.Text.Trim();
            var username = TBoxUsername.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                ShowMessage("Введите логин!", true);
                return;
            }

            if (username.Length < 3 || username.Length > 100)
            {
                ShowMessage("Логин должен быть от 3 до 100 символов!", true);
                return;
            }

            if (string.IsNullOrEmpty(email))
            {
                ShowMessage("Введите email!", true);
                return;
            }

            if (!IsValidEmail(email))
            {
                ShowMessage("Некорректный email!", true);
                return;
            }

            try
            {
                using var context = new FlowerShopDbContext();
                var user = context.Users.Find(_currentUser.Id);
                if (user != null)
                {
                    user.Username = username;
                    user.Email = email;
                    context.SaveChanges();
                    _currentUser.Email = email;
                    App.CurrentUser = _currentUser;
                }

                _isEditMode = false;
                TBoxEmail.IsEnabled = false;
                BtnEdit.Visibility = Visibility.Visible;
                BtnSave.Visibility = Visibility.Collapsed;
                BtnCancel.Visibility = Visibility.Collapsed;
                ShowMessage("Данные обновлены!", false);
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка: {ex.Message}", true);
            }
        }

        //Отмена
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _isEditMode = false;
            TBoxEmail.IsEnabled = false;
            TBoxEmail.Text = _currentUser.Email;
            BtnEdit.Visibility = Visibility.Visible;
            BtnSave.Visibility = Visibility.Collapsed;
            BtnCancel.Visibility = Visibility.Collapsed;
        }

        //Удаление профиля
        private void BtnDeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите удалить свой профиль?\nЭто действие нельзя отменить!",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using var context = new FlowerShopDbContext();
                    var user = context.Users.Find(_currentUser.Id);
                    if (user != null)
                    {
                        context.Users.Remove(user);
                        context.SaveChanges();

                        App.CurrentUser = null;
                        MessageBox.Show("Профиль удалён", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        NavigationService.Navigate(new AuthPage());
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage($"Ошибка удаления: {ex.Message}", true);
                }
            }
        }

        //Выход
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            NavigationService.Navigate(new AuthPage());
        }

       

        //Навигация
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AboutPage());
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Корзина в разработке", "Информация");
        private void BtnProfile_Click(object sender, RoutedEventArgs e) { }

        //Вспомогательные методы
        private void ShowMessage(string message, bool isError)
        {
            TxtMessage.Text = message;
            TxtMessage.Foreground = isError ? Brushes.Red : Brushes.Green;  
            TxtMessage.Visibility = Visibility.Visible;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    //Класс для отображения заказа
    public class OrderDisplayItem
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }  
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public int ItemsCount { get; set; }
    }
}