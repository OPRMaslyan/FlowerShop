using FlowerShop.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace FlowerShop.Pages
{
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
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

            using (var context = new FlowerShopDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == username);

                if (user == null)
                {
                    TxtError.Text = "Пользователь не найден!";
                    TxtError.Visibility = Visibility.Visible;
                    return;
                }

                // Проверка хэша пароля через BCrypt
                if (!BCrypt.Net.BCrypt.Verify(password, user.Passwordhash))
                {
                    TxtError.Text = "Неверный логин или пароль!";
                    TxtError.Visibility = Visibility.Visible;
                    return;
                }

                App.CurrentUser = user;
                NavigationService.Navigate(new MainMenuPage());
            }
        }

        private void PBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}