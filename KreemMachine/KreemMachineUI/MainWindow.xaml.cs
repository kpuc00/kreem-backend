using KreemMachine.ViewModels;
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
        ScheduledShift ManuallyScheduledShift;

        UserService userService = new UserService();
        ScheduleService scheduleService = new ScheduleService();
        ShiftService shiftService = new ShiftService();

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

        private void ScheduleManuallyButton_Click(object sender, EventArgs e)
        {
            ManuallyGenerateScheduleTabItem.IsSelected = true;
            ManualScheduleShiftPicker.AvailableShifts = shiftService.getAllShifts();

        }

        private void ManualScheduleShiftPicker_SelectedShiftChanged(object sender, DateTime SelectedDay, Shift SelectedShift)
        {
            
            ManuallyScheduledShift = scheduleService.GetScheduledShiftOrCreateNew(SelectedDay, SelectedShift);

            SetUpEmployeeRecomendationForManualScheduling();

        }

        private void SetUpEmployeeRecomendationForManualScheduling()
        {
            // TODO: Replace with Misho's algorithm for getting employee in recomended order

            var users = userService.GetAllByRole(Role.Employee).Select( u => new UserSchedulerViewModel(u, ManuallyScheduledShift));
            ManuallyScheduleUsersListBox.ItemsSource = users;
        }

        private void ShiftAssignmentCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var userViewModel = checkbox.DataContext as UserSchedulerViewModel;

            scheduleService.AssignUserToShift(userViewModel.User, ManuallyScheduledShift);
        }

        private void ShiftAssignmentCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var userViewModel = checkbox.DataContext as UserSchedulerViewModel;

            scheduleService.RemoveUserFromShift(userViewModel.User, ManuallyScheduledShift);

        }

        #endregion


    }
}
