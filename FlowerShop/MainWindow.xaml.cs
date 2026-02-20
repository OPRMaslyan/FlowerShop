using System.Windows;
using FlowerShop.Pages;

namespace FlowerShop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new AuthPage());
        }
    }
}