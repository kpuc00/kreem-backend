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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KreemMachine
{
    /// <summary>
    /// Interaction logic for MonthPicker.xaml
    /// </summary>
    public partial class MonthPicker : UserControl
    {
        public DateTime SelectedMonth { get; set; }

        public event EventHandler<DateTime> SelectedMonthChanged;

        public MonthPicker()
        {
            InitializeComponent();


            // The first day of the current month
            SelectedMonth = DateTime.Today.AddDays(-DateTime.Today.Day + 1); 
            DisplayDateTextBlock.Text = SelectedMonth.ToString("MMMM yyyy");
        }



        private void PreviousMonthButton_Click (object sender, EventArgs e)
        {
            SelectedMonth = SelectedMonth.AddMonths(-1);
            UpdateDate();
        }

        private void NextMonthButton_Click(object sender, EventArgs e)
        {
            SelectedMonth = SelectedMonth.AddMonths(1);
            UpdateDate();
        }

        private void UpdateDate()
        {
            SelectedMonthChanged?.Invoke(this, SelectedMonth);
            DisplayDateTextBlock.Text = SelectedMonth.ToString("MMMM yyyy");
        }
    }
}
