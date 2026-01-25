using FlowerShop.Models;
using System.Windows;
using System.Windows.Controls;
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
            if (string.IsNullOrWhiteSpace(TBoxLogin.Text) || string.IsNullOrWhiteSpace(PBoxPassword.Password))
            {
                TxtError.Text = "Введите логин и пароль!";
                TxtError.Visibility = Visibility.Visible;
                return;
            }

            using (var context = new FlowerShopDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == TBoxLogin.Text);
                if (user == null || user.Passwordhash != PBoxPassword.Password)
                {
                    TxtError.Text = "Неверный логин или пароль!";
                    TxtError.Visibility = Visibility.Visible;
                    return;
                }

                App.CurrentUser = user;
                NavigationService.Navigate(new MainMenuPage());
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Пока не реализуем регистрацию — задача только на авторизацию
            MessageBox.Show("Регистрация временно недоступна.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}