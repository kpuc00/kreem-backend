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
    public partial class RequestInfoGetterWindow : Window
    {
        public int? Quantity { get; set; } 

        public RequestInfoGetterWindow(string title ="Restock Request", string message ="Please enter the quantity", string buttonText = "Confirm")
        {
            InitializeComponent();
            this.DataContext = this;
            Title = title;
            MessageTextBlock.Text = message;
            ConfirmButton.Content = buttonText;
        }

        private void SendRequestButton_Click(object sender, RoutedEventArgs e)
        {
            if (uint.TryParse(ProductQuantityTextBox.Text, out uint quantity) == false)
                return;

            if (quantity == 0)
                return;

            Quantity = (int)quantity;


            this.DialogResult = true;

            this.Close();
        }

        private void ProductQuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        public bool? ShowDialog(out int result)
        {
            bool? dr = base.ShowDialog();
            result = Quantity ?? 0;
            return dr;
        }
    }
}
