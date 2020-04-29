using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KreemMachine
{
    /// <summary>
    /// Interaction logic for ScheduleShiftPicker.xamlA controll that allows the user to 
    /// pick a day and a shift related to that day. Raises an event everytime the data is changed.
    /// Requires a list of shifts and will preserve their order.
    /// </summary>
    public partial class ScheduleShiftPicker : UserControl
    {
        private IList<Shift> availableShifts;

        public IList<Shift> AvailableShifts
        {
            get => availableShifts;
            set
            {
                ShiftsComboBox.SelectedItem = value[0];
                ShiftsComboBox.ItemsSource = availableShifts = value;
            }
        }

        /// <summary>
        /// Gets and sets the Selected day of the calendar withour raising any events
        /// </summary>
        public DateTime SelectedDay
        {
            get => DisplayDayDatePicker.SelectedDate ?? default;
            set
            {
                DisplayDayDatePicker.SelectedDateChanged -= UI_SelectedDateChanged;
                DisplayDayDatePicker.SelectedDate = value;
                DisplayDayDatePicker.SelectedDateChanged += UI_SelectedDateChanged;

            }
        }

        /// <summary>
        /// Gets and sets the Selected shift of the Combobox and will raise an event 
        /// </summary>
        public Shift SelectedShift
        {
            get => ShiftsComboBox.SelectedItem as Shift;
            set => ShiftsComboBox.SelectedItem = value;
        }

        /// <summary>
        /// The time of the Event emitted by the class
        /// </summary>
        /// <param name="sender"> the object that emmits the event </param>
        /// <param name="SelectedDay"> the day selected at the time when the event is emitted</param>
        /// <param name="SelectedShift"> the shift selected at the time when the event is emitted </param>
        public delegate void ShiftPickerEventHandler(object sender, DateTime SelectedDay, Shift SelectedShift);

        /// <summary>
        /// Triggers every time the Selected Day or the Selected shift is changed via the UI
        /// </summary>
        public event ShiftPickerEventHandler SelectedShiftChanged;

        public ScheduleShiftPicker()
        {
            InitializeComponent();
            SelectedDay = DateTime.Now;

        }


        private void PreviousShiftButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShiftsComboBox.SelectedIndex == firstShiftIndex())
            {
                SelectedDay = previousDay();
                ShiftsComboBox.SelectedIndex = lastShiftIndex();
            }
            else
                ShiftsComboBox.SelectedIndex--;
            
        }

        int firstShiftIndex() => 0;
        int lastShiftIndex() => AvailableShifts.Count - 1;

        DateTime nextDay() => SelectedDay.AddDays(1);
        DateTime previousDay() => SelectedDay.AddDays(-1);

        private void NextShiftButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShiftsComboBox.SelectedIndex == lastShiftIndex())
            {
                SelectedDay = nextDay();
                ShiftsComboBox.SelectedIndex = firstShiftIndex();
            }
            else
                ShiftsComboBox.SelectedIndex++;
        }



        private void UI_SelectedDateChanged(object sender, SelectionChangedEventArgs e) =>
            SelectedShiftChanged?.Invoke(this, SelectedDay, SelectedShift);



    }
}
