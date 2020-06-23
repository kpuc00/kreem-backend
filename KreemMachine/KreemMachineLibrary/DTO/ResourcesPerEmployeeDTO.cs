using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.DTO
{
    public class ResourcesPerEmployeeDTO
    {
        public string EmployeeName { get; set; }
        public int NumberOfScheduledShifts { get; set; }
        public double HoursWorked { get; set; }
        public double Cost { get; set; }

        public ResourcesPerEmployeeDTO() { }
    }
}
