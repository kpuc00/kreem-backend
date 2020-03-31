using System;
using System.Collections.Generic;
using System.Linq;
using KreemMachineLibrary.Models;
using KreemMachineLibrary.Services;
using KreemMachineLibrary.Exceptions;
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
using System.Text.RegularExpressions;

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
            try
            {
                this.userService.UpdateEmployee(this.user, FirstNameTextBox.Text, LastNameTextBox.Text, EmailTextBox.Text, (Role)RoleComboBox.SelectedItem, HourlyWageTextBox.Text, (DateTime)BirthDatePicker.SelectedDate, AddressTextBox.Text, PhoneNumberTextBox.Text);

                this.Close();
            }
            catch (HourlyWageMustComtainOnlyNumbers ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (PhoneNumberException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (RequiredFieldsEmpty ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FirstNameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            string[] tempFirstLastName = CheckFirstLastNameTextBox();
            string firstName = tempFirstLastName[0];
            string lastName = tempFirstLastName[1];

            string employeeEmail = userService.GenerateEmployeeEmail(firstName, lastName);
            EmailTextBox.Text = employeeEmail;
        }

        private void LastNameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            
            string[] tempFirstLastName = CheckFirstLastNameTextBox();
            string firstName = tempFirstLastName[0];
            string lastName = tempFirstLastName[1];

            string employeeEmail = userService.GenerateEmployeeEmail(firstName, lastName);
            EmailTextBox.Text = employeeEmail;
        }

        private string[] CheckFirstLastNameTextBox()
        {
            string firstName = "example";
            string lastName = "example";
            string[] retVal = new string[2];

            if (!String.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                firstName = FirstNameTextBox.Text;
            }
            if (!String.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                lastName = LastNameTextBox.Text;
            }

            retVal[0] = firstName;
            retVal[1] = Regex.Replace(lastName, @"\s?", "");

            return retVal;
        }
    }
}
