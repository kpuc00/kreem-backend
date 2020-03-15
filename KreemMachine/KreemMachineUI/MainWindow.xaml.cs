﻿using KreemMachineLibrary.Models;
using KreemMachineLibrary.Services;
using KreemMachineLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
using System.Windows.Threading;

namespace KreemMachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<User> AllUsers;

        UserService userService = new UserService();

        User logedUSer;
        public MainWindow(User user)
        {
            InitializeComponent();
            logedUSer = user;
            

        }



        
        private void TabItem_Loaded(object sender, RoutedEventArgs e)
        {
            AllUsersListBox.ItemsSource = null;
            AllUsers = userService.GetAll();
            AllUsersListBox.ItemsSource = AllUsers;
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            AllUsersListBox.ItemsSource = userService.FilterEmployees(SearchTextBox.Text);
        }

        private void CreateUserButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new CreateUserWindow();
            window.Show();
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button.DataContext as User;
            try
            {
                int i = userService.DeleteEmployee(user, logedUSer);
                if (i == -1) {
                    Window window = new LoginWindow();
                    window.Show();

                    this.Close();
                }
            }
            catch (DeletAdminAccountException dex) {
                MessageBox.Show(dex.Message);
            }

        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button.DataContext as User;

            var window = new EditUserWindow(user, userService);
            window.Show();
        }

        private void TabItem_Selected(object sender, RoutedEventArgs e)
        {
            AllUsersListBox.ItemsSource = null;
            AllUsersListBox.ItemsSource = AllUsers; 
        }
    }
}
