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
    /// Interaction logic for CreateUserWindow.xaml
    /// </summary>
    public partial class CreateUserWindow : Window
    {

        UserService users = new UserService();

        public CreateUserWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RoleComboBox.ItemsSource = Enum.GetValues(typeof(Role));
            RoleComboBox.SelectedItem = Role.Employee;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var user = new User(
                FirstNameTextBox.Text,
                LastNameTextBox.Text,
                EmailTextBox.Text,
                (Role)RoleComboBox.SelectedItem,
                float.Parse(WageTextBox.Text),
                BirthDatePicker.SelectedDate,
                AdressTextBox.Text,
                PhoneTextBox.Text);

            users.Save(user);
        }
    }
}
