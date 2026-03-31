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

        public PaymentPage(int orderId, decimal amount)
        {
            InitializeComponent();
            _orderId = orderId;
            _amount = amount;
            _paymentMethod = "QR"; // По умолчанию QR-код
            LoadPaymentInfo();
        }

        private void LoadPaymentInfo()
        {
            TxtOrderInfo.Text = $"📦 Заказ №{_orderId}";
            TxtAmount.Text = $"{_amount:F2} ₽";
        }

        private void PaymentMethod_Checked(object sender, RoutedEventArgs e)
        {
            if (RadioQR.IsChecked == true)
            {
                QRSection.Visibility = Visibility.Visible;
                CashSection.Visibility = Visibility.Collapsed;
                _paymentMethod = "QR";
                BtnConfirmPayment.Content = "✅ Я оплатил";
                TxtHint.Text = "Отсканируйте QR-код и нажмите «Я оплатил»";
            }
            else if (RadioCash.IsChecked == true)
            {
                QRSection.Visibility = Visibility.Collapsed;
                CashSection.Visibility = Visibility.Visible;
                _paymentMethod = "Cash";
                BtnConfirmPayment.Content = "💵 Подтвердить";
                TxtHint.Text = "Оплата будет произведена при получении";
            }
        }

        private void BtnConfirmPayment_Click(object sender, RoutedEventArgs e)
        {
            if (_paymentMethod == "QR")
            {
                var result = MessageBox.Show(
                    "Подтвердите, что вы произвели оплату по QR-коду.\n\n" +
                    "После подтверждения статус заказа изменится на «Оплачен».",
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
                    "Вы выбрали оплату при получении.\n\n" +
                    "Статус заказа будет установлен как «Ожидает оплаты».\n" +
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
                        ? "Оплата подтверждена!\n\nСпасибо за покупку. Ваш заказ будет обработан в ближайшее время."
                        : "Заказ оформлен!\n\nОплатите заказ при получении. Мы свяжемся с вами для уточнения деталей.";

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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы действительно хотите отменить оплату?\n\n" +
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
    }
}