using KreemMachineLibrary.Extensions.Date;
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
    public partial class YearPicker : UserControl
    {
        public DateTime SelectedYear { get; set; }

        public event EventHandler<DateTime> SelectedYearChanged;

        public YearPicker()
        {
            InitializeComponent();

            SelectedYear = DateTime.Today.ThisYear();
            DisplayYearTextBlock.Text = SelectedYear.ToString("yyyy");
        }


        private void PreviousYearButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedYear = SelectedYear.AddYears(-1);
            UpdateDate();
        }

        private void NextYearButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedYear = SelectedYear.AddYears(1);
            UpdateDate();
        }

        private void UpdateDate()
        {
            SelectedYearChanged?.Invoke(this, SelectedYear);
            DisplayYearTextBlock.Text = SelectedYear.ToString("yyyy");
        }

        
    }
}
