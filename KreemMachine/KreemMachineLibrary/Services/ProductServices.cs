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

        public Product CreateProduct(string givenName, string givenBuyCost, string givenSellPrice, string givenQuantity, long givenDepartmentId)
        {
            Product product = ValidateInput(givenName, givenBuyCost, givenSellPrice, givenQuantity, givenDepartmentId);

            using (var db = new DataBaseContext())
            {
                product = db.Products.Add(product);
                db.SaveChanges();
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

        public int UpdateProduct(Product product, string productName, string buyCost, string sellPrice, string quantity, long departmentId)
        {
            ValidateInput(productName, buyCost, sellPrice, quantity, departmentId);

            product.Name = productName;
            product.BuyCost = float.Parse(buyCost);
            product.SellPrice = float.Parse(sellPrice);
            product.Quantity = int.Parse(quantity);
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

        private Product ValidateInput(string givenName, string givenBuyCost, string givenSellPrice, string givenQuantity, long givenDepartmentId)
        {
            if (string.IsNullOrWhiteSpace(givenName) || string.IsNullOrWhiteSpace(givenBuyCost) || string.IsNullOrWhiteSpace(givenSellPrice) || string.IsNullOrWhiteSpace(givenQuantity) || givenDepartmentId == 0)
            {
                throw new RequiredFieldsEmpty("You need to fill in all required fields");
            }

            else
            {
                //Checks if the buyCost and sellPrice is in correct format
                int dotCount = 0;
                bool valH = true;
                foreach (Char c in givenBuyCost)
                {
                    if (c == '.')
                    {
                        if (++dotCount > 1)
                        {
                            valH = false;
                            break;
                        }
                    }
                    else
                    {
                        if (c < '0' || c > '9')
                        {
                            valH = false;
                            break;
                        }
                    }
                }
                if (!valH)
                {
                    throw new BuyCostIncorrectFormatException("The buy cost value's format is incorrect!");
                }

                //Checks if the sellPrice and sellPrice is in correct format
                foreach (Char c in givenSellPrice)
                {
                    if (c == '.')
                    {
                        if (++dotCount > 1)
                        {
                            valH = false;
                            break;
                        }
                    }
                    else
                    {
                        if (c < '0' || c > '9')
                        {
                            valH = false;
                            break;
                        }
                    }
                }
                if (!valH)
                {
                    throw new SellPriceIncorrectFormatException("The sell price value is not in a correct format!");
                }

                foreach (Char c in givenQuantity)
                {
                    if (c < '0' || c > '9')
                    {
                        throw new QuantityIncorrectFormatException("The quantity value is not in a correct format!");
                    }
                }

                return new Product(givenName, int.Parse(givenQuantity), float.Parse(givenBuyCost), float.Parse(givenSellPrice), givenDepartmentId);
            }
        }
    }
}
