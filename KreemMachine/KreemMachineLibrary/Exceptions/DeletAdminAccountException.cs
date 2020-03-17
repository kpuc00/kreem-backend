using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Exceptions
{
    public class DeletAdminAccountException : Exception
    {
        public DeletAdminAccountException(string message) : base(message) { }
    }
}
