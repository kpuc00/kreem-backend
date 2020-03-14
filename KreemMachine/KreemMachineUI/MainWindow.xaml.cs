using KreemMachineLibrary.Models;
using KreemMachineLibrary.Services;
using KreemMachineLibrary;
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
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KreemMachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        ObservableCollection<User> AllUsers;
        IEnumerable<Shift> AllShifts;

        ConnectionSettingsDTO connectionSettingsDTO;
        public ConnectionSettingsDTO ConnectionSettings
        {
            get => connectionSettingsDTO;
            set
            {
                if (value != connectionSettingsDTO)
                {
                    connectionSettingsDTO = value;
                    NotifyPropertyChanged();
                }
            }
        }
        UserService userService = new UserService();
        ShiftService shiftService = new ShiftService();
        ConnectionSettingsService connectionService = new ConnectionSettingsService();

        public event PropertyChangedEventHandler PropertyChanged;

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

        private void ButtonSaveScheduleSetting_Click(object sender, RoutedEventArgs e)
        {
            shiftService.SaveChanges();

        }

        private void Settings_Tab_Loaded(object sender, RoutedEventArgs e)
        {
            AllShifts = shiftService.GetAllShifts();

            ShiftNameComboBox.ItemsSource = AllShifts;
        }

        private void ShiftTimeTextBox_TextChanged(object sender, RoutedEventArgs e)
        {

        }

        private void ShiftStartTextBox_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            HoursTextBlock.DataContext = null;
            HoursTextBlock.DataContext = ShiftNameComboBox.SelectedItem;
            Console.WriteLine("gg " + ((Shift)ShiftNameComboBox.SelectedItem).Duration);
        }

        private void ButtonSaveConnectionSetting_Click(object sender, RoutedEventArgs e)
        {
            connectionService.SaveChanges(ConnectionSettings);
        }

        private void TabItem_Selected(object sender, RoutedEventArgs e)
        {
            ConnectionSettings = connectionService.GetConnectionSettings();
        }


        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
