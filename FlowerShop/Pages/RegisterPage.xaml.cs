using BCrypt.Net;
using FlowerShop.Models;
using System;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FlowerShop.Pages
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        //Показ/скрытие подсказки для логина/email
        private void TBoxInput_TextChanged(object sender, TextChangedEventArgs e) { }

        //Показ/скрытие подсказки для пароля
        private void PBoxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TxtPasswordHint.Visibility = string.IsNullOrEmpty(PBoxPassword.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void PBoxPasswordConfirm_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TxtPasswordConfirmHint.Visibility = string.IsNullOrEmpty(PBoxPasswordConfirm.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var username = TBoxUsername.Text.Trim();
            var email = TBoxEmail.Text.Trim();
            var password = PBoxPassword.Password;
            var passwordConfirm = PBoxPasswordConfirm.Password;

            //Валидация
            if (string.IsNullOrEmpty(username))
            {
                ShowError("Введите логин!");
                TBoxUsername.Focus();
                return;
            }

            if (username.Length < 3 || username.Length > 100)
            {
                ShowError("Логин должен быть от 3 до 100 символов!");
                TBoxUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(email))
            {
                ShowError("Введите email!");
                TBoxEmail.Focus();
                return;
            }

            if (!IsValidEmail(email))
            {
                ShowError("Введите корректный email!");
                TBoxEmail.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Введите пароль!");
                PBoxPassword.Focus();
                return;
            }

            if (password.Length < 6)
            {
                ShowError("Пароль должен быть не менее 6 символов!");
                PBoxPassword.Focus();
                return;
            }

            if (password != passwordConfirm)
            {
                ShowError("Пароли не совпадают!");
                PBoxPasswordConfirm.Focus();
                return;
            }

            try
            {
                using var context = new FlowerShopDbContext();

                if (context.Users.Any(u => u.Username == username))
                {
                    ShowError("Пользователь с таким логином уже существует!");
                    TBoxUsername.Focus();
                    return;
                }

                if (context.Users.Any(u => u.Email == email))
                {
                    ShowError("Пользователь с таким email уже существует!");
                    TBoxEmail.Focus();
                    return;
                }

                //Хэширование
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                var newUser = new User
                {
                    Username = username,
                    Email = email,
                    Passwordhash = passwordHash,
                    Role = "Customer",
                    Createdat = DateTime.Now
                };

                context.Users.Add(newUser);
                context.SaveChanges();

                MessageBox.Show("Регистрация успешна! Теперь вы можете войти.", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.Navigate(new AuthPage());
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка регистрации: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void ShowError(string message)
        {
            TxtError.Text = message;
            TxtError.Visibility = Visibility.Visible;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void PBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}