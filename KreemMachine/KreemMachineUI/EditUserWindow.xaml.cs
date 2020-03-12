using System;
using System.Collections.Generic;
using System.Linq;
using KreemMachineLibrary.Models;
using KreemMachineLibrary.Services;
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
    /// Interaction logic for EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {
        User user;
        UserService userService;
        
        public EditUserWindow(User user, UserService userService)
        {
            InitializeComponent();
            this.user = user;
            this.userService = userService;

            FirstNameTextBox.Text = this.user.FirstName;
            LastNameTextBox.Text = this.user.LastName;
            EmailTextBox.Text = this.user.Email;
            RoleComboBox.ItemsSource = Enum.GetValues(typeof(Role));
            RoleComboBox.SelectedItem = this.user.Role;
            HourlyWageTextBox.Text = this.user.HourlyWage.ToString();
            BirthDatePicker.SelectedDate = (DateTime)this.user.Birthdate;
            AddressTextBox.Text = this.user.Address;
            PhoneNumberTextBox.Text = this.user.PhoneNumber;
        }

        private void UpdateUserButton_Click(object sender, RoutedEventArgs e)
        {
            this.user.FirstName = FirstNameTextBox.Text;
            this.user.LastName = LastNameTextBox.Text;
            this.user.Email = EmailTextBox.Text;
            this.user.Role = (Role)RoleComboBox.SelectedItem;
            this.user.HourlyWage = float.Parse(HourlyWageTextBox.Text);

            this.userService.UpdateEmployee(this.user);

            this.Close();
        }
    }
}
