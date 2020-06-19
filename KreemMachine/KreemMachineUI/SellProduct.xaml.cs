using KreemMachineLibrary.Models;
using KreemMachineLibrary.Services;
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
using System.Windows.Shapes;

namespace KreemMachine
{
    /// <summary>
    /// Interaction logic for SellProduct.xaml
    /// </summary>
    public partial class SellProduct : Window
    {
        Product product;
        ProductService productServices;

        public SellProduct(Product passedProduct, ProductService passedService)
        {
            InitializeComponent();

            product = passedProduct;
            productServices = passedService;

            NameTextBox.Text = product.Name;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            productServices.SellProduct(product, int.Parse(QuantityTextBox.Text), new ProductSale(int.Parse(QuantityTextBox.Text), DateTime.Now));

            this.Close();
        }
    }
}
