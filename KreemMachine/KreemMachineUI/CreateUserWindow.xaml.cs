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
using System.Text.RegularExpressions;
using KreemMachineLibrary.Exceptions;
using KreemMachineLibrary.Models.Statics;
using System.Collections.ObjectModel;

namespace KreemMachine
{
    /// <summary>
    /// Interaction logic for CreateUserWindow.xaml
    /// </summary>
    public partial class CreateUserWindow : Window
    {

        UserService users = new UserService();
        DepartmentService departmentService = new DepartmentService();

        public CreateUserWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RoleComboBox.ItemsSource = Enum.GetValues(typeof(Role));
            RoleComboBox.SelectedItem = Role.Employee;

            var departments = departmentService.GetAll();
            DepartmentComboBox.ItemsSource = departments;
            DepartmentComboBox.DisplayMemberPath = "Name";
            DepartmentComboBox.SelectedItem = departments[0];

            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            EmailTextBox.Text = "";
            HourlyWageTextBox.Text = "";
            BirthDatePicker.SelectedDate = null;
            AddressTextBox.Text = "";
            PhoneNumberTextBox.Text = "";

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                users.Save(FirstNameTextBox.Text, LastNameTextBox.Text, EmailTextBox.Text, (Role)RoleComboBox.SelectedItem, (Department)DepartmentComboBox.SelectedItem, HourlyWageTextBox.Text, BirthDatePicker.DisplayDate, AddressTextBox.Text, PhoneNumberTextBox.Text);

                this.Close();
            }
            catch (HourlyWageMustComtainOnlyNumbers ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (PhoneNumberException ex) {
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

            string employeeEmail = users.GenerateEmployeeEmail(firstName, lastName);
            EmailTextBox.Text = employeeEmail;
        }

        private void LastNameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            string[] tempFirstLastName = CheckFirstLastNameTextBox();
            string firstName = tempFirstLastName[0];
            string lastName = tempFirstLastName[1];

            string employeeEmail = users.GenerateEmployeeEmail(firstName, lastName);
            EmailTextBox.Text = employeeEmail;
        }

        private string[] CheckFirstLastNameTextBox() {
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
