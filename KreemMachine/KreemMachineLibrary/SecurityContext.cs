using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary
{
    public static class SecurityContext
    {
        public static User CurrentUser { get; private set; }

        internal static void Authenticate(User user)
        {
            CurrentUser = user;
        }
    }
}
