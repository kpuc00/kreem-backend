using KreemMachineLibrary.Exceptions;
using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Services
{
    public class ProductServices
    {
        public List<Product> GetViewableProducts()
        {
            using (var db = new DataBaseContext())
            {
                var products = db.Products.Include(p => p.Department);

                if (SecurityContext.HasPermissions(Permission.ViewAllProducts))
                    return products.ToList();

                if (SecurityContext.HasPermissions(Permission.ViewOwnProducts))
                {
                    long? usersDepartment = SecurityContext.CurrentUser.DepartmentId;
                    return products
                       .Where(p => p.DepartmentId == usersDepartment)
                       .ToList();
                }
                   

                throw new MissingPermissionEexception(Permission.ViewAllProducts, Permission.ViewOwnProducts);
            }
        }

        public void LoadProducts()
        {
            using (var db = new DataBaseContext())
                db.Products.Load();
        }
    }
}
