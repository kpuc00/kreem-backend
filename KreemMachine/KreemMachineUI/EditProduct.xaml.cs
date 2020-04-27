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
        ProductServices productServices;
        Department department;

        public EditProduct(Product givenProduct, ProductServices givenProductServices)
        {
            InitializeComponent();

            product = givenProduct;
            productServices = givenProductServices;

            var departments = productServices.GetAllDepartments();
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
            long departmentId = DepartmentComboBox.SelectedIndex + 1;
            productServices.UpdateProduct(product, ProductNameTextBox.Text, float.Parse(BuyCostTextBox.Text), float.Parse(SellPriceTextBox.Text), int.Parse(QuantityTextBox.Text), departmentId);
            this.Close();
        }
    }
}
