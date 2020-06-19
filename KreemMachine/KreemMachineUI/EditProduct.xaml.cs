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
    /// Interaction logic for EditProduct.xaml
    /// </summary>
    public partial class EditProduct : Window
    {
        Product product;
        ProductService productServices;
        DepartmentService departmentService = new DepartmentService();

        public EditProduct(Product givenProduct, ProductService givenProductServices)
        {
            InitializeComponent();

            product = givenProduct;
            productServices = givenProductServices;

            var departments = departmentService.GetAllViewable();
            DepartmentComboBox.DisplayMemberPath = "Name";
            DepartmentComboBox.ItemsSource = departments;

            ProductNameTextBox.Text = product.Name;
            BuyCostTextBox.Text = product.BuyCost.ToString();
            SellPriceTextBox.Text = product.SellPrice.ToString();
            QuantityTextBox.Text = product.Quantity.ToString();
            DepartmentComboBox.Text = product.Department.Name;
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
                Department selectedDepartment = (Department)DepartmentComboBox.SelectedItem;

                productServices.UpdateProduct(product, productName, buyCost, sellPrice, quantity, selectedDepartment);

                this.Close();
            }
            catch (BuyCostIncorrectFormatException ex)
            {
                MessageBox.Show(ex.Message, "Edit product", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (SellPriceIncorrectFormatException ex)
            {
                MessageBox.Show(ex.Message, "Edit product", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (QuantityIncorrectFormatException ex)
            {
                MessageBox.Show(ex.Message, "Edit product", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (RequiredFieldsEmpty ex)
            {
                MessageBox.Show(ex.Message, "Edit product", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
