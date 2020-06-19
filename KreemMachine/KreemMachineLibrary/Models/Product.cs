using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Models
{
    [Table("product")]
    public class Product
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Column("buy_cost"),Required]
        public float BuyCost{ get; set; }

        [Column("sell_price"), Required]
        public float SellPrice { get; set; }

        [Column("department_id"), Required]
        public long DepartmentId { get; set; }

        [Column("deleted")]
        public int Deleted { get; set; }

        public virtual Department Department { get; set; }

        public Product() { }

        public Product(string name, int quantity, float buyCost, float sellPrice, Department department)
        {
            Name = name;
            Quantity = quantity;
            BuyCost = buyCost;
            SellPrice = sellPrice;
            DepartmentId = department.Id;
        }
    }
}
