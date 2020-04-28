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
using KreemMachineLibrary.Exceptions;

namespace KreemMachine
{
    /// <summary>
    /// Interaction logic for CreateProduct.xaml
    /// </summary>
    public partial class CreateProduct : Window
    {
        ProductServices productServices = new ProductServices();

        public CreateProduct()
        {
            InitializeComponent();

            var departments = productServices.GetAllDepartments();
            DepartmentComboBox.ItemsSource = departments;
            DepartmentComboBox.DisplayMemberPath = "Name";

            ProductNameTextBox.Text = "";
            BuyCostTextBox.Text = "";
            SellPriceTextBox.Text = "";
            QuantityTextBox.Text = "";
            DepartmentComboBox.Text = "";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string productName = ProductNameTextBox.Text;
                string buyCost = BuyCostTextBox.Text;
                string sellPrice = SellPriceTextBox.Text;
                string quantity = QuantityTextBox.Text;
                int departmentId = DepartmentComboBox.SelectedIndex + 1;

                productServices.CreateProduct(productName, buyCost, sellPrice, quantity, departmentId);

                this.Close();
            }
            catch (BuyCostIncorrectFormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (SellPriceIncorrectFormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (QuantityIncorrectFormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (RequiredFieldsEmpty ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
