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

        public Product CreateProduct(string givenName, float givenBuyCost, float givenSellPrice, int givenQuantity, long givenDepartmentId)
        {
            Product product = new Product(givenName, givenQuantity, givenBuyCost, givenSellPrice, givenDepartmentId);
            using (var db = new DataBaseContext())
            {
                product = db.Products.Add(product);
            }

            return product;
        }

        public int RemoveProduct(Product product)
        {
            using (var db = new DataBaseContext())
            {
                if (!db.Products.Local.Contains(product))
                {
                    db.Products.Attach(product);
                    db.Products.Remove(product);
                    db.SaveChanges();
                }
            }

            return 1;
        }

        public int UpdateProduct(Product product, string productName, float buyCost, float sellPrice, int quantity, long departmentId)
        {
            product.Name = productName;
            product.BuyCost = buyCost;
            product.SellPrice = sellPrice;
            product.Quantity = quantity;
            product.DepartmentId = departmentId;

            using (var db = new DataBaseContext())
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }

            return 1;
        }

        public List<Department> GetAllDepartments()
        {
            using (var db = new DataBaseContext())
            {
                return db.Departments.ToList();
            }
        }

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
