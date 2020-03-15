using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Exceptions
{
    public class RequiredFieldsEmpty : Exception
    {
        public RequiredFieldsEmpty(string message) : base(message) { }
    }
}
