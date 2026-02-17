using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlowerShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для FlowersCatalogPage.xaml
    /// </summary>
    public partial class FlowersCatalogPage : Page
    {
        public FlowersCatalogPage()
        {
            InitializeComponent();
            LoadFlowers();
        }

        private void LoadFlowers()
        {
            using (var context = new FlowerShopDbContext())
            {
                var flowers = context.Flowers
                     .Include(f => f.Category) 
                     .ToList();
                ItemsControlFlowers.ItemsSource = flowers;
            }
        }
        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Как нибудь потом", "Будущее", MessageBoxButton.OKCancel);
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FlowersCatalogPage());
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AboutPage());
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Как нибудь потом", "Будущее", MessageBoxButton.OKCancel);
        }
        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Как нибудь потом", "Будущее", MessageBoxButton.OKCancel);
        }
    }
}
