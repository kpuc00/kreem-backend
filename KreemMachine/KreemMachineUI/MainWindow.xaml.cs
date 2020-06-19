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
using KreemMachine.UserControls;
using KreemMachineLibrary.Extensions.Date;

namespace KreemMachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        IEnumerable<Shift> AllShifts;

        #region Security Bindings
        public bool CanViewUsersTab => SecurityContext.HasPermissions(Permission.ViewAllUsers)
                                        || SecurityContext.HasPermissions(Permission.ViewOwnUsers);
        public bool CanViewScheduleTab => SecurityContext.HasPermissions(Permission.ViewSchedule);
        public bool CanViewStatisticsTab => SecurityContext.HasPermissions(Permission.ViewStatistics);
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
        public bool CanViewDepartmentsTab => SecurityContext.HasPermissions(Permission.ViewDeparments);
        public bool CanAddDepartments => SecurityContext.HasPermissions(Permission.CreateDepartments);
        public bool CanEditDepartments => SecurityContext.HasPermissions(Permission.EditDepartments);
        public bool CanDeleteDepartments => SecurityContext.HasPermissions(Permission.DeleteDepartments);
        public bool CanChangeSettings => SecurityContext.HasPermissions(Permission.ChangeSettings);
        public bool CanViewSettingsTab => SecurityContext.HasPermissions(Permission.ViewSettings);
        public bool CanAutogenerateSchedule => SecurityContext.HasPermissions(Permission.AutogenerateSchedule);
        public bool CanCreateProducts => SecurityContext.HasPermissions(Permission.CreateProducts);
        public bool CanSellProducts => SecurityContext.HasPermissions(Permission.SellOwnProducts);
        public bool CanEditProducts => SecurityContext.HasPermissions(Permission.EditOwnProducts);
        public bool CanDeleteProducts => SecurityContext.HasPermissions(Permission.DeleteProducts);
        #endregion

        public string ServerField => HostTextBox.Text;
        public string UsernameField => ConnectionUsernameTextBox.Text;
        public string PasswordField => ConnectionPasswordPasswordBox.Password;
        public string DatabaseNameField => ConnectionDatabaseNameTextBox.Text;

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
        ProductService productServices = new ProductService();
        StockService stockService = new StockService();
        DepartmentService departmentService = new DepartmentService();

        public event PropertyChangedEventHandler PropertyChanged;

        Timer RefreshUsersViewTimer = new Timer(5000);
        Timer RefreshProductsTableTimer = new Timer(5000);
        Timer RefreshDepartmentTableTimer = new Timer(5000);

        LoginWindow loginWindow;

        public MainWindow(User user, LoginWindow loginWindow)
        {
            InitializeComponent();
            fromDatePicker.SelectedDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            toDatePicker.SelectedDate = DateTime.Now.Date;
            RefreshUsersViewTimer.Elapsed += (sender, e) => Dispatcher.Invoke(() => RefreshUsersTableView());
            RefreshProductsTableTimer.Elapsed += (sender, e) => Dispatcher.Invoke(() => RefreshProductsTable());
            RefreshDepartmentTableTimer.Elapsed += (sender, e) => Dispatcher.Invoke(() => RefreshDepartmentTable());

            AllDepartmentsListBox.ItemsSource = departmentService.GetAllViewable();

            this.loginWindow = loginWindow;
            this.Closed += MainWindow_Closed;
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            AllUsersListBox.ItemsSource = userService.FilterEmployees(SearchTextBox.Text);
            if (!string.IsNullOrEmpty(SearchTextBox.Text))
            {
                RefreshUsersViewTimer.Stop();
            }
            else
            {
                RefreshUsersViewTimer.Start();
            }
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
            connectionService.ChangeConnectionString(ServerField, UsernameField, PasswordField, DatabaseNameField);
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
                    int i = userService.DeleteEmployee(user, SecurityContext.CurrentUser);
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
            AllUsersListBox.ItemsSource = userService.GetUsers();
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

        private async void AutogenerateScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            AutogenerateScheduleButton.IsEnabled = false;

            var tasks = new List<Task>();

            DateTime monthStart = DateTime.Now.AddMonths(1).ThisMonth();

            for (DateTime weekToProcess = monthStart;
                weekToProcess <= monthStart.AddMonths(1);
                weekToProcess = weekToProcess.AddDays(7))
            {
                DateTime weekStart = weekToProcess;
                DateTime weekEnd = weekToProcess.AddDays(7);
                Console.WriteLine($"start: {weekStart} end: {weekEnd}");

                tasks.Add(Task.Run(() =>
               {
                   List<ScheduledShift> shifts = scheduleService.GetOrCreateScheduledShifts(weekStart, weekEnd);
                   scheduleService.AutogenerateSchedule(shifts);
               }));
                //break;
            }

            await Task.WhenAll(tasks.ToArray());
            AutogenerateScheduleButton.IsEnabled = true;
            MessageBox.Show("Schedule generation completed ");

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
            RefreshProductsTable();
            RefreshProductsTableTimer.Start();
        }

        private void StockTabItem_Unselected(object sender, RoutedEventArgs e)
        {
            RefreshProductsTableTimer.Stop();
        }

        private void SearchProductsBar_KeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchProductsBar.Text))
            {
                RefreshProductsTableTimer.Stop();
                AllProductsListBox.ItemsSource = productServices.FilterProducts(SearchProductsBar.Text);
            }
            else
            {
                RefreshProductsTable(sender, e);
                RefreshProductsTableTimer.Start();
            }
        }

        private void AddNewProductButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new CreateProduct();
            window.Show();
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button.DataContext as Product;

            var window = new EditProduct(product, productServices);
            window.Show();
            SearchProductsBar.Text = null;
            RefreshProductsTable(sender, e);
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshProductsTableTimer.Stop();
            if (MessageBox.Show("You are about to remove this product. Continue?", "Delete product", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var button = sender as Button;
                var product = button.DataContext as Product;

                int i = productServices.RemoveProduct(product);
                if (i == 1)
                {
                    SearchProductsBar.Text = null;
                    StocKTabItem_Selected(sender, e);
                }
                RefreshProductsTableTimer.Start();
            }
        }

        private void RefreshProductsTable(object sender = null, EventArgs args = null)
        {
            Console.WriteLine("Refreshing products");
            AllProductsListBox.ItemsSource = productServices.GetViewableProducts();
        }

        private void RequestRestockForProductButton_Clicked(object sender, RoutedEventArgs e)
        {
            var product = ((Button)sender).DataContext as Product;
            var form = new RequestInfoGetterWindow(
                title: "Create stock request",
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
            var form = new RequestInfoGetterWindow(
                title: "Approve stock request",
                message: "Please specify how many items you want to approve",
                buttonText: "Approve request");

            if (form.ShowDialog(out int quantity) == true)
            {
                stockService.ApproveRequest(request, quantity);
                await UpdateRestockRequestsTab();
            }

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

        #region Stock-Stats
        private void SellProductButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button.DataContext as Product;

            var form = new SellProduct(product, productServices);
            form.Show();

            //Console.WriteLine(statisticsService.GetMostSellingProduct());
            //Console.WriteLine(statisticsService.GetLeastSellingProduct());
            //Console.WriteLine(statisticsService.CalculateProfit(product));
            Console.WriteLine(statisticsService.GetMostProfitableProduct());
            Console.WriteLine(statisticsService.GetLeastProfitableProduct());
        }

        private void DateStock_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            StockStatistics();
        }

        private void StockCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StockStatistics();
        }

        private void StockStatistics()
        {
            if (cbxStockCategory.SelectedItem == null)
            {
                return;
            }
            if (rbnStockPrice.IsChecked == true)
            {
                StockGrid.ItemsSource = statisticsService.GetIncomeThisMonth(startDateStock.SelectedDate ?? default(DateTime), endDateStock.SelectedDate ?? default(DateTime), (Department)cbxStockCategory.SelectedItem);
            }
            else if (rbnStockAmount.IsChecked == true)
            {
                StockGrid.ItemsSource = statisticsService.GetAmountSoldThisMonth(startDateStock.SelectedDate ?? default(DateTime), endDateStock.SelectedDate ?? default(DateTime), (Department)cbxStockCategory.SelectedItem);
            }
        }

        private void StockStatisticsTab_Selected(object sender, RoutedEventArgs e)
        {
            var departments = departmentService.GetAllViewable();
            cbxStockCategory.ItemsSource = departments;
            cbxStockCategory.SelectedItem = departments[0];
            cbxStockCategory.DisplayMemberPath = "Name";

            startDateStock.SelectedDate = DateTime.Now.ThisMonth();
            endDateStock.SelectedDate = DateTime.Now.NextMonth();

            StockStatistics();


        }

        private void StockPrice_Checked(object sender, RoutedEventArgs e)
        {
            StockStatistics();
        }

        #endregion

        #region Departments

        private void btnAddDepartment_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddDepartmentWindow(departmentService);
            window.Show();
        }


        private void btnDeleteDepartment_Click(object sender, RoutedEventArgs e)
        {
            RefreshDepartmentTableTimer.Stop();
            if (MessageBox.Show("You are about to remove this department. Continue?", "Delete department", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var button = sender as Button;
                var department = button.DataContext as Department;

                int i = departmentService.Delete(department);
                if (i == 1)
                {
                    SearchDepartmentsBar.Text = null;
                    DepartmentTab_Selected(sender, e);
                }
                RefreshDepartmentTableTimer.Start();
            }
        }

        private void btnEditDepartment_Click_1(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var department = button.DataContext as Department;

            var window = new EditDepartmentWindow(departmentService, department);
            window.Show();
        }

        private void DepartmentTab_Selected(object sender, RoutedEventArgs e)
        {
            RefreshDepartmentTable();
            RefreshDepartmentTableTimer.Start();
        }

        private void DepartmentTab_Unselected(object sender, RoutedEventArgs e)
        {
            RefreshDepartmentTableTimer.Stop();
        }

        private void RefreshDepartmentTable()
        {
            AllDepartmentsListBox.ItemsSource = null;
            AllDepartmentsListBox.ItemsSource = departmentService.GetAllViewable();
        }

        private void SearchDepartmentsBar_KeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchDepartmentsBar.Text))
            {
                RefreshDepartmentTableTimer.Stop();
                AllDepartmentsListBox.ItemsSource = departmentService.FilterDepartments(SearchDepartmentsBar.Text);
            }

            else
            {
                DepartmentTab_Selected(sender, e);
            }
        }
        #endregion

        private void LogoutBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.loginWindow.Show();

                this.Closed -= MainWindow_Closed;

                this.Close();
            }
            MainContent.SelectedItem = UsersTabItem;
        }

        private void MainWindow_Closed(object sender, EventArgs e) => loginWindow.Close();
    }
}
