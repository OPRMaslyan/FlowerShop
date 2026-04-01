using FlowerShop.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlowerShop.Pages
{
    public partial class PaymentPage : Page
    {
        private int _orderId;
        private decimal _amount;
        private string _paymentMethod;

        // Конструктор страницы
        public PaymentPage(int orderId, decimal amount)
        {
            InitializeComponent();
            _orderId = orderId;
            _amount = amount;
            _paymentMethod = "QR";
            LoadPaymentInfo();
        }

        // Загрузка информации о платеже
        private void LoadPaymentInfo()
        {
            TxtOrderInfo.Text = $"Заказ номер {_orderId}";
            TxtAmount.Text = $"{_amount:F2} ₽";
        }

        // Переключение между способами оплаты
        private void PaymentMethod_Checked(object sender, RoutedEventArgs e)
        {
            if (RadioQR.IsChecked == true)
            {
                QRSection.Visibility = Visibility.Visible;
                CashSection.Visibility = Visibility.Collapsed;
                _paymentMethod = "QR";
                BtnConfirmPayment.Content = "Я оплатил";
                TxtHint.Text = "Отсканируйте QR-код и нажмите Я оплатил";
            }
            else if (RadioCash.IsChecked == true)
            {
                QRSection.Visibility = Visibility.Collapsed;
                CashSection.Visibility = Visibility.Visible;
                _paymentMethod = "Cash";
                BtnConfirmPayment.Content = "Подтвердить";
                TxtHint.Text = "Оплата будет произведена при получении";
            }
        }

        // Подтверждение оплаты
        private void BtnConfirmPayment_Click(object sender, RoutedEventArgs e)
        {
            if (_paymentMethod == "QR")
            {
                var result = MessageBox.Show(
                    "Подтвердите, что вы произвели оплату по QR-коду." + Environment.NewLine + Environment.NewLine +
                    "После подтверждения статус заказа изменится на Оплачен.",
                    "Подтверждение оплаты",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    UpdateOrderStatus("Paid");
                }
            }
            else if (_paymentMethod == "Cash")
            {
                var result = MessageBox.Show(
                    "Вы выбрали оплату при получении." + Environment.NewLine + Environment.NewLine +
                    "Статус заказа будет установлен как Ожидает оплаты." + Environment.NewLine +
                    "Оплатите заказ курьеру при получении.",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    UpdateOrderStatus("Pending");
                }
            }
        }

        // Обновление статуса заказа в базе данных
        private void UpdateOrderStatus(string status)
        {
            try
            {
                using var context = new FlowerShopDbContext();
                var order = context.Orders.FirstOrDefault(o => o.Id == _orderId);

                if (order != null)
                {
                    order.Status = status;
                    context.SaveChanges();

                    string message = _paymentMethod == "QR"
                        ? "Оплата подтверждена!" + Environment.NewLine + Environment.NewLine +
                          "Спасибо за покупку. Ваш заказ будет обработан в ближайшее время."
                        : "Заказ оформлен!" + Environment.NewLine + Environment.NewLine +
                          "Оплатите заказ при получении. Мы свяжемся с вами для уточнения деталей.";

                    string title = _paymentMethod == "QR" ? "Успех" : "Заказ оформлен";

                    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new ProfilePage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при подтверждении оплаты: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Отмена оплаты и заказа
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы действительно хотите отменить оплату?" + Environment.NewLine + Environment.NewLine +
                "Заказ будет отменён.",
                "Отмена оплаты",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using var context = new FlowerShopDbContext();
                    var order = context.Orders.FirstOrDefault(o => o.Id == _orderId);
                    if (order != null)
                    {
                        order.Status = "Cancelled";
                        context.SaveChanges();
                    }
                }
                catch { }

                NavigationService.Navigate(new ProfilePage());
            }
        }

        // Навигация по меню
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new FlowersCatalogPage());
        private void BtnAbout_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AboutPage());
        private void BtnMenu_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenuPage());
        private void BtnCart_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new CartPage());
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ProfilePage());
    }
}