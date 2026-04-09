using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FlowerShop.Pages
{
    public partial class AuthPage : Page
    {
        // Конструктор страницы
        public AuthPage()
        {
            InitializeComponent();
        }

        // Обработчик изменения текста логина
        private void TBoxLogin_TextChanged(object sender, TextChangedEventArgs e) { }

        // Обработчик изменения пароля
        private void PBoxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TxtPasswordHint.Visibility = string.IsNullOrEmpty(PBoxPassword.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // Обработчик нажатия клавиш в поле пароля
        private void PBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
            if (e.Key == Key.Enter) BtnLogin_Click(sender, e);
        }

        // Вход в аккаунт
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = TBoxLogin.Text.Trim();
            var password = PBoxPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TxtError.Text = "Введите логин и пароль";
                TxtError.Visibility = Visibility.Visible;
                return;
            }

            using var context = new FlowerShopDbContext();
            var user = context.Users.Include(u => u.Role).FirstOrDefault(u => u.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Passwordhash))
            {
                TxtError.Text = "Неверный логин или пароль";
                TxtError.Visibility = Visibility.Visible;
                return;
            }

            App.CurrentUser = user;

            if (user.Role != null && user.Role.Name == "Admin")
                NavigationService.Navigate(new AdminPanelPage());
            else
                NavigationService.Navigate(new MainMenuPage());
        }

        // Переход к регистрации
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}