using KreemMachineLibrary;
using KreemMachineLibrary.Models;
using KreemMachineLibrary.Models.Statics;
using KreemMachineLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for CreateRestockRequestWindow.xaml
    /// </summary>
    public partial class CreateRestockRequestWindow : Window
    {
        Product Product;
        StockService stockService = new StockService();

        public CreateRestockRequestWindow(Product product)
        {
            InitializeComponent();
            Product = product;

            ProductNameTextBlock.Text = Product.Name;
        }

        private void SendRequestButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ProductQuantityTextBox.Text, out int quantity) == false)
                return;

            RestockRequest request = new RestockRequest(Product);

            RestockStage openStage = new RestockStage()
            {
                Quantity = quantity,
                Request = request,
                Status = RestockStageType.Open,
                User = SecurityContext.CurrentUser,
            };

            stockService.CreateRequestFromOpenStage(openStage);
            this.Close();
        }

        private void ProductQuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
