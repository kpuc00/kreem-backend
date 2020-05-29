using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Exceptions
{
    class DepartmentExistsException : Exception
    {
        public DepartmentExistsException(string message) : base(message) { }
    }
}
