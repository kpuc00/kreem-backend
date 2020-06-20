using KreemMachineLibrary.Exceptions;
using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Services
{
    public class ProductService
    {
        public Product CreateProduct(string givenName, string givenBuyCost, string givenSellPrice, string givenQuantity, Department selectedDepartment)
        {
            Product product = ValidateInput(givenName, givenBuyCost, givenSellPrice, givenQuantity, selectedDepartment);

            using (var db = new DataBaseContext())
            {
                product = db.Products.Add(product);
                db.SaveChanges();
            }

            return product;
        }

        public int RemoveProduct(Product product)
        {
            product.Deleted = 1;

            using (var db = new DataBaseContext())
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }

            return 1;
        }

        public int UpdateProduct(Product product, string productName, string buyCost, string sellPrice, string quantity, Department selectedDepartment)
        {
            ValidateInput(productName, buyCost, sellPrice, quantity, selectedDepartment);

            product.Name = productName;
            product.BuyCost = float.Parse(buyCost);
            product.SellPrice = float.Parse(sellPrice);
            product.Quantity = int.Parse(quantity);
            product.DepartmentId = selectedDepartment.Id;
            product.Department = null;

            using (var db = new DataBaseContext())
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }

            return 1;
        }

        internal void UpdateProduct(Product product, ProductSale soldProduct, int quantity)
        {
            var sProduct = new ProductSale(quantity, DateTime.Now, product);

            if (product.Quantity < soldProduct.Quantity)
                throw new InvalidOperationException("Cannot sell more products than are available in stock");

            product.Quantity -= soldProduct.Quantity;


            using (var db = new DataBaseContext())
            {
                soldProduct = db.ProductSales.Add(sProduct);
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void SellProduct(Product product, int quantity, ProductSale soldProduct)
        {
            UpdateProduct(product, soldProduct, quantity);
        }

        public List<Product> GetViewableProducts()
        {
            using (var db = new DataBaseContext())
            {
                var products = db.Products.Include(p => p.Department);

                if (SecurityContext.HasPermissions(Permission.ViewAllProducts))
                    return products
                        .Where(p => p.Deleted == 0)
                        .ToList();

                if (SecurityContext.HasPermissions(Permission.ViewOwnProducts))
                {
                    long? usersDepartment = SecurityContext.CurrentUser.DepartmentId;
                    return products
                       .Where(p => p.DepartmentId == usersDepartment && p.Deleted == 0)
                       .ToList();
                }


                throw new MissingPermissionException(Permission.ViewAllProducts, Permission.ViewOwnProducts);
            }
        }

        public void LoadProducts()
        {
            using (var db = new DataBaseContext())
                db.Products.Load();
        }

        private Product ValidateInput(string givenName, string givenBuyCost, string givenSellPrice, string givenQuantity, Department givenDepartment)
        {
            if (string.IsNullOrWhiteSpace(givenName) || string.IsNullOrWhiteSpace(givenBuyCost) || string.IsNullOrWhiteSpace(givenSellPrice) || string.IsNullOrWhiteSpace(givenQuantity) || givenDepartment == null)
            {
                throw new RequiredFieldsEmpty("You need to fill in all the required fields!");
            }

            else
            {
                //Checks if the buyCost is in correct format
                int dotCount = 0;
                bool valH = true;
                foreach (Char c in givenBuyCost)
                {
                    if (c == ',')
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
                    throw new BuyCostIncorrectFormatException("The 'Buy cost' value's format is incorrect!");
                }
                dotCount = 0;
                valH = true;
                //Checks if the sellPrice is in correct format
                foreach (Char c in givenSellPrice)
                {
                    if (c == ',')
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
                    throw new SellPriceIncorrectFormatException("The 'Sell price' value's format is incorrect!");
                }

                foreach (Char c in givenQuantity)
                {
                    if (c < '0' || c > '9')
                    {
                        throw new QuantityIncorrectFormatException("The 'Quantity' value's format is incorrect!");
                    }
                }

                return new Product(givenName, int.Parse(givenQuantity), float.Parse(givenBuyCost), float.Parse(givenSellPrice), givenDepartment);
            }
        }

        public List<Product> FilterProducts(string s)
        {
            using (var db = new DataBaseContext())
            {
                var products = db.Products.Include(p => p.Department);

                if (SecurityContext.HasPermissions(Permission.ViewAllProducts))
                    return products
                        .Where(p => p.Name.ToLower().Contains(s.ToLower()) && p.Deleted == 0)
                        .ToList();

                if (SecurityContext.HasPermissions(Permission.ViewOwnProducts))
                {
                    long? usersDepartment = SecurityContext.CurrentUser.DepartmentId;
                    return products
                        .Where(p => p.DepartmentId == usersDepartment && p.Name.ToLower().Contains(s.ToLower()) && p.Deleted == 0)
                       .ToList();
                }

                throw new MissingPermissionException(Permission.ViewAllProducts, Permission.ViewOwnProducts);
            }
        }
    }
}
