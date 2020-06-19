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
using System.Data.Entity.Infrastructure;

namespace KreemMachine
{
    /// <summary>
    /// Interaction logic for CreateProduct.xaml
    /// </summary>
    public partial class CreateProduct : Window
    {
        ProductService productServices = new ProductService();
        DepartmentService departmentService = new DepartmentService();

        public CreateProduct()
        {
            InitializeComponent();

            var departments = departmentService.GetAllViewable();
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
                Department selectedDepartment = (Department)DepartmentComboBox.SelectedItem;

                productServices.CreateProduct(productName, buyCost, sellPrice, quantity, selectedDepartment);

                this.Close();
            }
            catch (BuyCostIncorrectFormatException ex)
            {
                MessageBox.Show(ex.Message, "Create product", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (SellPriceIncorrectFormatException ex)
            {
                MessageBox.Show(ex.Message, "Create product", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (QuantityIncorrectFormatException ex)
            {
                MessageBox.Show(ex.Message, "Create product", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (RequiredFieldsEmpty ex)
            {
                MessageBox.Show(ex.Message, "Create product", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Something went wrong. Make sure the name has not been used before!", "Create product", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
