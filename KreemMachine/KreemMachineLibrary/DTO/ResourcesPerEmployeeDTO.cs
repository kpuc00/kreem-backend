using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.DTO
{
    public class ResourcesPerEmployeeDTO
    {
        public string name { get; set; }
        public int shiftCount { get; set; }
        public double hourSum { get; set; }
        public float empCost { get; set; }

        public ResourcesPerEmployeeDTO() { }
    }
}
