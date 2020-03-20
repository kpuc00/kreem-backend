using KreemMachine.ViewModels;
using KreemMachineLibrary.Models;
using KreemMachineLibrary.Services;
using KreemMachineLibrary;
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
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using KreemMachineLibrary.DTO;

namespace KreemMachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        ObservableCollection<User> AllUsers;

        IEnumerable<Shift> AllShifts;

        public bool CanViewUsersTab => SecurityContext.HasPermissions(Permission.ViewUsers);
        public bool CanViewScheduleTab => SecurityContext.HasPermissions(Permission.ViewSchedule);
        public bool CanViewStatisticsTab => SecurityContext.HasPermissions(Permission.ViewSchedule);
        public bool CanCreateUser => SecurityContext.HasPermissions(Permission.CreateUsers);
        public bool CanEditUser => SecurityContext.HasPermissions(Permission.EditUsers);
        public bool CanDeleteUser => SecurityContext.HasPermissions(Permission.DeleteUsers);
        public bool CanEditSchedule => SecurityContext.HasPermissions(Permission.EditSchedule);


        ScheduledShift manuallyScheduledShift;

        public ScheduledShift ManuallyScheduledShift
        {
            get => manuallyScheduledShift;
            set
            {
                manuallyScheduledShift = value;
                NotifyPropertyChanged();
            }
        }

        private int selectedStafForSchedulingBinding;

        public int SelectedStafForSchedulingBinding
        {
            get => selectedStafForSchedulingBinding;
            set
            {
                selectedStafForSchedulingBinding = value;
                NotifyPropertyChanged();
            }
        }

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
        ScheduleService scheduleService = new ScheduleService();
        StatisticsService statisticsService = new StatisticsService();

        public event PropertyChangedEventHandler PropertyChanged;

        User logedUSer;
        public MainWindow(User user)
        {
            InitializeComponent();
            logedUSer = user;
            fromDatePicker.SelectedDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            toDatePicker.SelectedDate = DateTime.Now.Date;
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

        private void ButtonSaveScheduleSetting_Click(object sender, RoutedEventArgs e)
        {
            shiftService.SaveChanges();

        }

        private void Settings_Tab_Loaded(object sender, RoutedEventArgs e)
        {
            AllShifts = shiftService.GetAllShifts();

            ShiftNameComboBox.ItemsSource = AllShifts;
        }

        private void ShiftStartTextBox_TargetUpdated(object sender, EventArgs e)
        {

            var shift = ShiftNameComboBox.SelectedItem as Shift;

            try
            {
                shift.StartHour = TimeSpan.Parse(ShiftStartTextBox.Text);
                shift.EndHour = TimeSpan.Parse(ShiftEndTextBox.Text);
            }
            catch (FormatException) { }

            HoursTextBlock.DataContext = null;
            HoursTextBlock.DataContext = ShiftNameComboBox.SelectedItem;
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
            if (MessageBox.Show("Are you sure you want to delete this user?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var button = sender as Button;
                var user = button.DataContext as User;
                try
                {
                    int i = userService.DeleteEmployee(user, logedUSer);
                    if (i == -1)
                    {
                        Window window = new LoginWindow();
                        window.Show();

                        this.Close();
                    }
                }
                catch (DeletAdminAccountException dex)
                {
                    MessageBox.Show(dex.Message);
                }
            }
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button.DataContext as User;

            var window = new EditUserWindow(user, userService);
            window.Show();
        }

        private void UsersTabItem_Selected(object sender, RoutedEventArgs e)
        {
            AllUsersListBox.ItemsSource = null;
            AllUsersListBox.ItemsSource = AllUsers;
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

            void addMonthDaysToCallendarDays()
            {
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
        private void ScheduleTab_Selected(object sender, RoutedEventArgs e)
        {
            ScheduleMonthPicker_SelectedMonthChanged(sender, ScheduleMonthPicker.SelectedMonth);
        }


        private void ScheduleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1)
                return;

            var selected = e.AddedItems[0] as ScheduleDayViewModel;

            ScheduleManuallyButton_Click(null, null);
            ManualScheduleShiftPicker.SelectedDay = selected.Day;

        }

        private void ScheduleManuallyButton_Click(object sender, EventArgs e)
        {
            ManuallyGenerateScheduleTabItem.IsSelected = true;
            ManualScheduleShiftPicker.AvailableShifts = shiftService.GetAllShifts();

        }

        private void ManualScheduleShiftPicker_SelectedShiftChanged(object sender, DateTime SelectedDay, Shift SelectedShift)
        {

            ManuallyScheduledShift = scheduleService.GetScheduledShiftOrCreateNew(SelectedDay, SelectedShift);
            SelectedStafForSchedulingBinding = ManuallyScheduledShift?.EmployeeScheduledShits?.Count ?? 0;
            SetUpEmployeeRecomendationForManualScheduling();

        }

        private void SetUpEmployeeRecomendationForManualScheduling()
        {
            // TODO: Replace with Misho's algorithm for getting employee in recomended order

            var users = userService.GetAllByRole(Role.Employee).Select(u => new UserSchedulerViewModel(u, ManuallyScheduledShift));

            ManuallyScheduleUsersListBox.ItemsSource = users;
        }

        private void ShiftAssignmentCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var userViewModel = checkbox.DataContext as UserSchedulerViewModel;
            scheduleService.AssignUserToShift(userViewModel.User, ManuallyScheduledShift);
            SelectedStafForSchedulingBinding++;

        }

        private void ShiftAssignmentCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var userViewModel = checkbox.DataContext as UserSchedulerViewModel;

            scheduleService.RemoveUserFromShift(userViewModel.User, ManuallyScheduledShift);
            SelectedStafForSchedulingBinding--;

        }

        #endregion

        #region Statistics

        //Resources per shift

        private void ResPerShiftTab_Loaded(object sender, RoutedEventArgs e)
        {
            StatisticsPerShiftMonthPicker_SelectedMonthChanged(sender, StatisticsPerShiftMonthPicker.SelectedMonth);
        }

        private void StatisticsPerShiftMonthPicker_SelectedMonthChanged(object sender, DateTime displayMonth)
        {
            string cbMorning = "";
            string cbNoon = "";
            string cbNight = "";
            if ((bool)cbMorningShift.IsChecked)
            {
                cbMorning = cbMorningShift.Content.ToString();
            }
            if ((bool)cbNoonShift.IsChecked)
            {
                cbNoon = cbNoonShift.Content.ToString();
            }
            if ((bool)cbNightShift.IsChecked)
            {
                cbNight = cbNightShift.Content.ToString();
            }

            if (String.IsNullOrWhiteSpace(cbMorning) && String.IsNullOrWhiteSpace(cbNoon) && String.IsNullOrWhiteSpace(cbNight))
            {
                ResPerShiftDataGrid.ItemsSource = statisticsService.GetResourcesPerShift(displayMonth);
            }
            else
            {
                ResPerShiftDataGrid.ItemsSource = statisticsService.GetResourcesPerShift(displayMonth, cbMorning, cbNoon, cbNight);
            }
        }

        private void cbShift_CheckChange(object sender, RoutedEventArgs e)
        {
            StatisticsPerShiftMonthPicker_SelectedMonthChanged(sender, StatisticsPerShiftMonthPicker.SelectedMonth);
        }

        //Resources per month

        private void ResPerMonthTab_Loaded(object sender, RoutedEventArgs e)
        {
            StatisticsPerMonthYearPicker_SelectedYearChanged(sender, StatisticsPerMonthYearPicker.SelectedYear);
        }

        private void StatisticsPerMonthYearPicker_SelectedYearChanged(object sender, DateTime displayYear)
        {
            ResPerMonthDataGrid.ItemsSource = statisticsService.GetResourcesPerMonth(displayYear);
        }

        //Employee statistics

        private void EmplStatsTab_Loaded(object sender, RoutedEventArgs e)
        {
            EmplStatsDataGrid.ItemsSource = statisticsService.GetResourcesPerEmployee();
        }

        private void fromDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            EmplStatsDataGrid.ItemsSource = statisticsService.GetResourcesPerEmployeeDate(fromDatePicker.SelectedDate ?? default(DateTime), toDatePicker.SelectedDate ?? default(DateTime));
        }

        private void toDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            EmplStatsDataGrid.ItemsSource = statisticsService.GetResourcesPerEmployeeDate(fromDatePicker.SelectedDate ?? default(DateTime), toDatePicker.SelectedDate ?? default(DateTime));
        }


        #endregion

        
    }
}
