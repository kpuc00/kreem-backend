using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.DTO
{
    public class ResourcesPerShiftDTO
    {
        public int empCount { get; set; }
        public float empCost { get; set; }
        public DateTime date { get; set; }
        public string shift { get; set; }

        public ResourcesPerShiftDTO() { }

    }
}
