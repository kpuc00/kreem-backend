using KreemMachineLibrary.Models;
using KreemMachineLibrary.Models.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KreemMachineLibrary.Models.Statics.Role;
using static KreemMachineLibrary.Permission;

namespace KreemMachineLibrary
{
    public static class SecurityContext
    {
        public static User CurrentUser { get; private set; }

        static readonly Dictionary<Role, Permission[]> PermissionTable = new Dictionary<Role, Permission[]> {
            { UserAdmin, new []{
                ViewAllUsers,
                ViewOwnUsers,
                CreateUsers,
                EditUsers,
                DeleteUsers,
                ViewSchedule,
                EditSchedule,
                AutogenerateSchedule,
                ScheduleAnyEmployee,
                ScheduleOwnEmployee,
                ViewStatistics,
                EditShifts,
                ViewSettings,
                ChangeSettings,
                LogIn,
                } 
            },
            { ProductAdmin, new[] {
                ViewAllProducts,
                ViewOwnProducts,
                CreateProducts,
                DeleteProducts,
                EditOwnProducts,
                ViewAllRestockRequests,
                ChangeRestockRequests,
                LogIn,
                }
            },
            { DepartmentAdmin, new[]{
                ViewAllUsers,
                CreateDepartments,
                ViewDeparments,
                EditDepartments,
                DeleteDepartments,
                ViewStatistics,
                LogIn,
                } 
            },
            { Manager, new[] {
                ViewOwnUsers,
                ViewSchedule,
                EditSchedule,
                AutogenerateSchedule,
                ViewStatistics,
                ViewOwnProducts,
                RequestRestockForOwnProduct,
                ViewOwnRestockRequests,
                SellOwnProducts,
                ScheduleOwnEmployee,
                LogIn,
                }
            },
            
            { Employee, Array.Empty<Permission>() },
        };

        internal static void Authenticate(User user)
        {
            CurrentUser = user;
        }

        /// <summary>
        /// Returns true if user has all given permission and false otherwise
        /// </summary>
        public static bool HasPermissions(params Permission[] permissions)
        {
            if (CurrentUser != null && CurrentUser.Role.HasValue == false) return false;

            return PermissionTable[CurrentUser.Role.Value].Intersect(permissions).Count() == permissions.Length;
        }

        public static bool HasAny(params Permission[] permissions)
        {
            if (CurrentUser != null && CurrentUser.Role.HasValue == false) return false;

            return PermissionTable[CurrentUser.Role.Value].Intersect(permissions).Count() > 0;
        }

        /// <summary>
        /// Returns true if user has either of given roles
        /// </summary>
        public static bool HasRole(params Role?[] roles) => roles.Contains(CurrentUser.Role);

    }
}
