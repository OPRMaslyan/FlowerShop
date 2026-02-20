using System.Windows;
using FlowerShop.Models;

namespace FlowerShop
{
    public partial class App : Application
    {
        public static User? CurrentUser { get; set; }
    }
}