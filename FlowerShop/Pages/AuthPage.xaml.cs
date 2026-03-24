using FlowerShop.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FlowerShop.Pages.AdminPanel;

namespace FlowerShop.Pages
{
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void TBoxLogin_TextChanged(object sender, TextChangedEventArgs e) { }

        private void PBoxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TxtPasswordHint.Visibility = string.IsNullOrEmpty(PBoxPassword.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void PBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
            if (e.Key == Key.Enter) BtnLogin_Click(sender, e);
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = TBoxLogin.Text.Trim();
            var password = PBoxPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TxtError.Text = "Введите логин и пароль!";
                TxtError.Visibility = Visibility.Visible;
                return;
            }

            using var context = new FlowerShopDbContext();
            var user = context.Users.FirstOrDefault(u => u.Username == username);

            // 👇 Исправлено: полный путь к методу Verify
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Passwordhash))
            {
                TxtError.Text = "Неверный логин или пароль!";
                TxtError.Visibility = Visibility.Visible;
                return;
            }

            App.CurrentUser = user;

            if (user.Role == "Admin" || user.Role == "admin")
                NavigationService.Navigate(new AdminPanelPage());
            else
                NavigationService.Navigate(new MainMenuPage());
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}