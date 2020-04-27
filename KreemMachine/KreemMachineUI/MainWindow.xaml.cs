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
using System.Timers;

namespace KreemMachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        IEnumerable<Shift> AllShifts;

        public bool CanViewUsersTab => SecurityContext.HasPermissions(Permission.ViewUsers);
        public bool CanViewScheduleTab => SecurityContext.HasPermissions(Permission.ViewSchedule);
        public bool CanViewStatisticsTab => SecurityContext.HasPermissions(Permission.ViewSchedule);
        public bool CanCreateUser => SecurityContext.HasPermissions(Permission.CreateUsers);
        public bool CanEditUser => SecurityContext.HasPermissions(Permission.EditUsers);
        public bool CanDeleteUser => SecurityContext.HasPermissions(Permission.DeleteUsers);
        public bool CanEditSchedule => SecurityContext.HasPermissions(Permission.EditSchedule);
        public bool CanViewProductsTab => SecurityContext.HasPermissions(Permission.ViewAllProducts)
                                          || SecurityContext.HasPermissions(Permission.ViewOwnProducts);
        public bool CanViewRestockRequestsTab => SecurityContext.HasPermissions(Permission.ViewRestockRequests);
        public bool CanRequestProductRestock => SecurityContext.HasPermissions(Permission.RequestRestockForAnyProduct)
                                          || SecurityContext.HasPermissions(Permission.RequestRestockForOwnProduct);
        public bool CanChangeRestockRequests => SecurityContext.HasPermissions(Permission.ChangeRestockRequests);

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

        private int scheduleGeneratorCurrentDayNumberOfEmployees;

        public int ScheduleGeneratorCurrentDayNumberOfEmployees
        {
            get => scheduleGeneratorCurrentDayNumberOfEmployees;
            set
            {
                scheduleGeneratorCurrentDayNumberOfEmployees = value;
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
        ProductServices productServices = new ProductServices();
        StockService stockService = new StockService();

        public event PropertyChangedEventHandler PropertyChanged;

        User logedUSer;

        Timer RefreshUsersViewTimer = new Timer(5000);

        public MainWindow(User user)
        {
            InitializeComponent();
            logedUSer = user;
            fromDatePicker.SelectedDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            toDatePicker.SelectedDate = DateTime.Now.Date;
            RefreshUsersViewTimer.Elapsed += (sender, e) => Dispatcher.Invoke(() => RefreshUsersTableView());

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
            var shift = ShiftNameComboBox.SelectedItem as Shift;
            shiftService.Save(shift);
        }

        private void Settings_Tab_Selected(object sender, RoutedEventArgs e)
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
            RefreshUsersTableView();
            RefreshUsersViewTimer.Start();
        }

        private void UsersTabItem_Unselected(object sender, RoutedEventArgs e)
        {
            RefreshUsersViewTimer.Stop();
        }

        private void RefreshUsersTableView(object sender = null, EventArgs args = null)
        {
            Console.WriteLine("Refreshing users");
            AllUsersListBox.ItemsSource = null;
            AllUsersListBox.ItemsSource = userService.GetAll();
        }


        #region Schedule

        private async void ScheduleMonthPicker_SelectedMonthChanged(object sender, DateTime displayMonth)
        {
            await UpdateCalendarView(displayMonth);
        }

        private async Task UpdateCalendarView(DateTime displayMonth)
        {
            var calendarDays = new List<ScheduleDayViewModel>();

            addNullPaddingToCallendarDays();
            addMonthDaysToCallendarDays();
            await matchScheduledShiftsToCallendarDays();

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

            async Task matchScheduledShiftsToCallendarDays()
            {

                List<ScheduledShift> getScheduledShifts = await scheduleService.GetScheduledShiftsAsync(
                    start: displayMonth,
                    end: displayMonth.AddMonths(1)
                );

                IEnumerator<ScheduledShift> scheduledShifts = getScheduledShifts.GetEnumerator();
                IEnumerator<ScheduleDayViewModel> daysOfMonth = calendarDays.GetEnumerator();


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

        private async void ScheduleTab_Selected(object sender, RoutedEventArgs e)
        {
            await UpdateCalendarView(ScheduleMonthPicker.SelectedMonth);
        }


        private void ScheduleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1)
                return;

            var selected = e.AddedItems[0] as ScheduleDayViewModel;

            ManualScheduleShiftPicker.SelectedDay = selected.Day;
            ScheduleManuallyButton_Click(null, null);
           // ManualScheduleShiftPicker_SelectedShiftChanged(this, selected.Day, ManualScheduleShiftPicker.SelectedShift);

        }

        private void ScheduleManuallyButton_Click(object sender, EventArgs e)
        {
            ManuallyGenerateScheduleTabItem.IsSelected = true;
            ManualScheduleShiftPicker.AvailableShifts = shiftService.GetAllShifts();

        }

        private async void ManualScheduleShiftPicker_SelectedShiftChanged(object sender, DateTime SelectedDay, Shift SelectedShift)
        {

            ManuallyScheduledShift = await scheduleService.GetScheduledShiftOrCreateNewAsync(SelectedDay, SelectedShift);
            ScheduleGeneratorCurrentDayNumberOfEmployees = ManuallyScheduledShift?.EmployeeScheduledShits?.Count ?? 0;
            SetUpEmployeeRecomendationForManualScheduling();

        }

        private void SetUpEmployeeRecomendationForManualScheduling()
        {

            var users = scheduleService.GetSuggestedEmployees(ManuallyScheduledShift).Select(u => new UserSchedulerViewModel(u, ManuallyScheduledShift));
            ManuallyScheduleUsersListBox.ItemsSource = users;
        }

        private async void ShiftAssignmentCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var userViewModel = checkbox.DataContext as UserSchedulerViewModel;
            await scheduleService.AssignUserToShiftAsync(userViewModel.User, ManuallyScheduledShift);
            ScheduleGeneratorCurrentDayNumberOfEmployees++;

        }

        private async void ShiftAssignmentCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var userViewModel = checkbox.DataContext as UserSchedulerViewModel;

            await scheduleService.RemoveUserFromShiftAsync(userViewModel.User, ManuallyScheduledShift);
            ScheduleGeneratorCurrentDayNumberOfEmployees--;

        }

        #endregion

        #region Statistics

        //Resources per shift

        private void ResPerShiftTab_Selected(object sender, RoutedEventArgs e)
        {
            StatisticsPerShiftMonthPicker_SelectedMonthChanged(sender, StatisticsPerShiftMonthPicker.SelectedMonth);
        }

        private void StatisticsPerShiftMonthPicker_SelectedMonthChanged(object sender, DateTime displayMonth)
        {
            string cbMorning = null;
            string cbNoon = null;
            string cbNight = null;
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

        private void ResPerShiftDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == nameof(ResourcesPerShiftDTO.Date))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
            }
        }

        //Resources per month

        private void ResPerMonthTab_Selected(object sender, RoutedEventArgs e)
        {
            StatisticsPerMonthYearPicker_SelectedYearChanged(sender, StatisticsPerMonthYearPicker.SelectedYear);
        }

        private void StatisticsPerMonthYearPicker_SelectedYearChanged(object sender, DateTime displayYear)
        {
            ResPerMonthDataGrid.ItemsSource = statisticsService.GetResourcesPerMonth(displayYear);
        }

        private void ResPerMonthDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == nameof(ResourcesPerMonthDTO.Month))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "MMMM";
        }

        //Employee statistics

        private void EmplStatsTab_Selected(object sender, RoutedEventArgs e)
        {
            EmplStatsDataGrid.ItemsSource = statisticsService.GetResourcesPerEmployeeDate(fromDatePicker.SelectedDate ?? default(DateTime), toDatePicker.SelectedDate ?? default(DateTime));
        }

        private void fromDatePicker_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            EmplStatsDataGrid.ItemsSource = statisticsService.GetResourcesPerEmployeeDate(fromDatePicker.SelectedDate ?? default(DateTime), toDatePicker.SelectedDate ?? default(DateTime));
        }

        #endregion

        #region Products

        private void StocKTabItem_Selected(object sender, RoutedEventArgs e)
        {
            AllProductsListBox.ItemsSource = productServices.GetViewableProducts();
        }

        private void RequestRestockForProductButton_Clicked(object sender, RoutedEventArgs e)
        {
            var product = ((Button)sender).DataContext as Product;
            var form = new RequestInfoGetterWindow(
                title: "Create stock Request",
                message: "Please specify how many items you want to request",
                buttonText: "Open request");

            if (form.ShowDialog(out int quantity) == true)
            {
                RestockRequest request = new RestockRequest(product);
                stockService.CreateRequestFromOpenStage(request, quantity);
            }
        }

        #endregion

        #region Restock

        private async void RestockRequestsTab_Selected(object sender, RoutedEventArgs e)
        {
            await UpdateRestockRequestsTab();
        }

        private async Task UpdateRestockRequestsTab()
        {
            List<RestockRequest> requests = await stockService.GetActiveRequestsAsync();
            List<RestockRequestViewModel> viewModels = requests.Select(r => new RestockRequestViewModel(r)).ToList();
            viewModels.ForEach(SetUpEventsForRestockRequestViewModel);

            RestockRequestsItemsComponent.ItemsSource = viewModels;
            ICollectionView view = CollectionViewSource.GetDefaultView(viewModels);
            view.Filter = FilterRestockRequestByTextBoxInput;

        }

      

        private void SetUpEventsForRestockRequestViewModel(RestockRequestViewModel viewModel)
        {
            viewModel.RequestApproved += RestockRequestApproved;
            viewModel.RequestDenied += RestockRequestDenied;
            viewModel.RequestRestocked += RestocRequestRestocked;
            viewModel.RequestHidden += RestockRequestHidden;
        }

        private async void RestockRequestApproved(object sender, RestockRequest request)
        {
            stockService.ApproveRequest(request, request.LatestStage.Quantity ?? 0);
            await UpdateRestockRequestsTab();
        }

        private async void RestockRequestDenied(object sender, RestockRequest request)
        {
            stockService.DenyRequest(request);
            await UpdateRestockRequestsTab();
        }

        private async void RestocRequestRestocked(object sender, RestockRequest request)
        {
            stockService.RestockRequest(request, request.LatestStage.Quantity ?? 0);
            await UpdateRestockRequestsTab();
        }

        private async void RestockRequestHidden(object sender, RestockRequest request)
        {
            stockService.HideRequest(request);
            await UpdateRestockRequestsTab();
        }

        private bool FilterRestockRequestByTextBoxInput(object product)
        {
            return string.IsNullOrEmpty(SearchRestockRequestTextBox.Text) || requestContainsSearchwords();

            bool requestContainsSearchwords()
            {
                RestockRequest request = ((RestockRequestViewModel)product).Request;
                string productName = request.Product.Name;
                string productDepartment = request.Product.Department.Name;
                string membersName = request.Stages
                    .Select(s => s.TypeStr + s.Date.ToString("yyyy-mm-dd") + s.User.FullName)
                    .Aggregate(string.Concat);
                string searchSource = (productName + productDepartment + membersName).ToLower();

                string[] searchSequence = SearchRestockRequestTextBox.Text.ToLower().Split(' ');

                return searchSequence.Any(search => searchSource.Contains(search));
            }
        }

        private void SearchRestockRequestTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(RestockRequestsItemsComponent.ItemsSource);
            view.Refresh();
        }




        #endregion

    }
}
