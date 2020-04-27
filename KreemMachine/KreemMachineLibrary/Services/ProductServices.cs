using KreemMachineLibrary.Models;
using KreemMachineLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity;

using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Services
{
    public class ProductServices
    {
        DataBaseContext db = Globals.db;

        public Product CreateProduct(string givenName, float givenBuyCost, float givenSellPrice, int givenQuantity, long givenDepartmentId)
        {
            Product product = new Product(givenName, givenQuantity, givenBuyCost, givenSellPrice, givenDepartmentId);
            product = db.Products.Add(product);
            return product;
        }

        public void RemoveProduct(Product product)
        {
            if (db.Products.Local.Contains(product))
            {
                db.Products.Attach(product);
                db.Products.Remove(product);
                db.SaveChanges();
            }
        }

        public void UpdateProduct(Product product, string productName, float buyCost, float sellPrice, int quantity, long departmentId)
        {
            product.Name = productName;
            product.BuyCost = buyCost;
            product.SellPrice = sellPrice;
            product.Quantity = quantity;
            product.DepartmentId = departmentId;

            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

        }

        public List<Product> DisplayAllProducts()
        {
            return db.Products.ToList();
        }

        public List<Department> GetAllDepartments()
        {
            return db.Departments.ToList();
        }

    }
}
