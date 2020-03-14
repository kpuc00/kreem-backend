using KreemMachine.ViewModels;
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
        ScheduleService scheduleService = new ScheduleService();

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
        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button.DataContext as User;

            userService.DeleteEmployee(user);

        }


        #region Schedule

        private void ScheduleMonthPicker_SelectedMonthChanged(object sender, DateTime displayMonth)
        {

            var calendarDays = new List<ScheduleDayViewModel>();

            addNullPaddingToCallendarDays();
            addMonthDaysToCallendarDays();
            matchScheduledShiftsToCallendarDays();
            
            ScheduleListBox.ItemsSource = calendarDays;

            void addNullPaddingToCallendarDays()
            {
                // Will add necessary number of nulls to the list so that the first non-null element's position
                // matches that day's position in the calendar (monday - first, tuesday - second and so on)
                // It would be nice to have a better way to add paddings but for know this is it

                // DayOfWeek returns 0 for sunday, 1 for monday, 2 for tuesday and so on
                int skips = (int)displayMonth.DayOfWeek - 1;
                skips = (skips == -1) ? 6 : skips;

                for (int i = 0; i < skips; i++)
                    calendarDays.Add(null);
            }

            void addMonthDaysToCallendarDays(){
                for (var day = displayMonth; day < displayMonth.AddMonths(1); day = day.AddDays(1))
                    calendarDays.Add(new ScheduleDayViewModel(day));
            }

            void matchScheduledShiftsToCallendarDays()
            {
                IEnumerator<ScheduleDayViewModel> daysOfMonth = calendarDays.GetEnumerator();
                IEnumerator<ScheduledShift> scheduledShifts = scheduleService.GetScheduledShifts(
                    start: displayMonth,
                    end: displayMonth.AddMonths(1)
                    ).GetEnumerator();

                scheduledShifts.MoveNext();
                while (scheduledShifts.Current != null)
                {
                    if (scheduledShifts.Current.Date == daysOfMonth?.Current?.Day)
                    {
                        daysOfMonth.Current.Shifts.Add(scheduledShifts.Current);
                        scheduledShifts.MoveNext();
                    }
                    else
                        daysOfMonth.MoveNext();

                }
            }

        }
        private void ScheduleTab_Loaded(object sender, RoutedEventArgs e)
        {
            ScheduleMonthPicker_SelectedMonthChanged(sender, ScheduleMonthPicker.SelectedMonth);
        }


        private void ScheduleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1)
                return;

            var selected = e.AddedItems[0] as ScheduleDayViewModel;

            foreach(var shift in selected.Shifts)
            {
                Console.WriteLine(shift.Shift.Name, shift.Date);
            }
            Console.WriteLine();
        }



        #endregion


    }
}
