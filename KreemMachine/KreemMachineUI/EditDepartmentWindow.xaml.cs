using KreemMachineLibrary.Models;
using KreemMachineLibrary.Services;
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

namespace KreemMachine
{
    /// <summary>
    /// Interaction logic for EditDepartmentWindow.xaml
    /// </summary>
    public partial class EditDepartmentWindow : Window
    {

        DepartmentService departmentService;
        Department department;

        public EditDepartmentWindow(DepartmentService departmentService, Department department)
        {
            InitializeComponent();
            this.departmentService = departmentService;
            this.department = department; 
            tbxDepartmentName.Text = department.Name;
        }

        private void btnEditDepartment_Click(object sender, RoutedEventArgs e)
        {
            department.Name = tbxDepartmentName.Text;
            this.departmentService.Update(department);
            this.Close();
        }
    }
}
