﻿using KreemMachine.ViewModels;
using KreemMachineLibrary.Models;
using KreemMachineLibrary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<User> AllUsers;

        UserService userService = new UserService();

        public MainWindow()
        {
            InitializeComponent();
        }


        private void TabItem_Loaded(object sender, RoutedEventArgs e)
        {
            AllUsers = userService.GetAll();
            AllUsersListBox.ItemsSource = AllUsers;
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

            userService.DeleteEmployee(user);

        }


        #region Schedule

        private void ScheduleMonthPicker_SelectedMonthChanged(object sender, DateTime month)
        {

            var x = new List<ScheduleDayViewModel>();


            int skips = (int)month.DayOfWeek - 1;
            skips = (skips == -1) ? 6 : skips;

            for (int i = 0; i < skips; i++)
            {
                x.Add(default);
            }

            for (var day = month; day < month.AddMonths(1); day = day.AddDays(1))
                x.Add(new ScheduleDayViewModel(day));

            

            ScheduleListBox.ItemsSource = x;

        }
        private void ScheduleTab_Loaded(object sender, RoutedEventArgs e)
        {
            ScheduleMonthPicker_SelectedMonthChanged(sender, ScheduleMonthPicker.SelectedMonth);
        }


        #endregion


    }
}
