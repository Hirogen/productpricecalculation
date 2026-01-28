using System.Windows;
using ProductPriceCalculator.ViewModels;

namespace ProductPriceCalculator.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
