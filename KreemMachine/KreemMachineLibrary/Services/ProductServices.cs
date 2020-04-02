using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Services
{
    public class ProductServices
    {
        DataBaseContext db = Globals.db;

        public List<Product> GetAllProducts()
        {
            return db.Products.ToList();
        }

    }
}
