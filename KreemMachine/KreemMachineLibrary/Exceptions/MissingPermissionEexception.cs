using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Exceptions
{
    class MissingPermissionEexception : Exception
    {
        public MissingPermissionEexception(params Permission[] permissions)
            :base("You don't have the right permission to perform this action")
        {
            this.Data["allowed"] = permissions;
        }

        public override string ToString()
        {
            string permissions = string.Join(", ", Data["allowed"]);
            return base.ToString() + "\n You need one of the following permissions: " + permissions;
        }
    }
}
