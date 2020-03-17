using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KreemMachineLibrary.Models.Role;
using static KreemMachineLibrary.Permission;

namespace KreemMachineLibrary
{
    public static class SecurityContext
    {
        public static User CurrentUser { get; private set; }

        static readonly Dictionary<Role, Permission[]> PermissionTable = new Dictionary<Role, Permission[]> {
            { Administrator, new[] { ViewUsers, CreateUsers, EditUsers, DeleteUsers, ViewSchedule, EditSchedule, AutogenerateSchedule, ViewStatistics, EditShifts,  } },
            { Manager, new[] { ViewUsers, ViewSchedule, EditSchedule, AutogenerateSchedule, ViewStatistics, EditShifts,  } },
            { Depot, new[] {ViewSchedule} },
            { Employee, new Permission[]{} },
        };

        internal static void Authenticate(User user)
        {
            CurrentUser = user;
        }

        public static bool HasPermissions(params Permission[] permissions)
        {
            if (!CurrentUser.Role.HasValue) return false;

            return PermissionTable[CurrentUser.Role.Value].Intersect(permissions).Count() == permissions.Length;
        }

        public static bool HasRole(Role? role) =>
            CurrentUser.Role == role;
        
    }
}
