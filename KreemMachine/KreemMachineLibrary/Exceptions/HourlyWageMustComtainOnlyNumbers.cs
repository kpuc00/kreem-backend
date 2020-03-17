using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Exceptions
{
    public class HourlyWageMustComtainOnlyNumbers : Exception
    {
        public HourlyWageMustComtainOnlyNumbers(string message) : base(message) { }
    }
}
