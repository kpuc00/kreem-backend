using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.DTO
{
    public class ResourcesPerEmployeeDTO
    {
        public string Employee { get; set; }
        public int Shifts { get; set; }
        public double HoursWorked { get; set; }
        public float Cost { get; set; }

        public ResourcesPerEmployeeDTO() { }
    }
}
