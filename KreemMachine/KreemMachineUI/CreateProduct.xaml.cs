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
        Department department = new Department();

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
            long departmentId = DepartmentComboBox.SelectedIndex + 1;
            productServices.CreateProduct(ProductNameTextBox.Text, float.Parse(BuyCostTextBox.Text), float.Parse(SellPriceTextBox.Text), int.Parse(QuantityTextBox.Text), departmentId);
            this.Close();
        }
    }
}
