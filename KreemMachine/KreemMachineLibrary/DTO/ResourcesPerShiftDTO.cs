using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.DTO
{
    public class ResourcesPerShiftDTO
    {
        public DateTime Date { get; set; }
        public string Shift { get; set; }
        public int NumberOfEmployees { get; set; }
        public double Cost { get; set; }        

        public ResourcesPerShiftDTO() { }

    }
}
