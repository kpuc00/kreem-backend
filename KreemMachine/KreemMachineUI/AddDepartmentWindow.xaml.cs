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
using System.Windows.Shapes;
using KreemMachineLibrary.Models;
using KreemMachineLibrary.Services;

namespace KreemMachine
{
    /// <summary>
    /// Interaction logic for AddDepartmentWindow.xaml
    /// </summary>
    public partial class AddDepartmentWindow : Window
    {
        DepartmentService departmentService;
        public AddDepartmentWindow(DepartmentService departmentService)
        {
            InitializeComponent();
            this.departmentService = departmentService;
        }

        private void btnAddDEpartment_Click(object sender, RoutedEventArgs e)
        {
            Department department = new Department(tbxDepartmentName.Text);
            this.departmentService.SaveToDatabase(department);

            this.Close();
        }
    }
}
