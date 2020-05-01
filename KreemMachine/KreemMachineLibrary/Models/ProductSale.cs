using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Models
{
    [Table("product_sale")]
    public class ProductSale
    {
        [Key]
        public long Id { get; set; }

        [Column("quantity"), Required]
        public int Quantity { get; set; }

        [Column("timestamp"), Required]
        public DateTime? Timestamp { get; set; } = DateTime.Now;

        [Column("product_id"), Required]
        public long ProductId { get; set; }

        public virtual Product Product { get; set; }

        public ProductSale(int quantity, DateTime? timestamp)
        {
            Quantity = quantity;
            Timestamp = timestamp;
        }

        public ProductSale(int quantity, DateTime? timestamp, Product product) : this(quantity, timestamp)
        {
            ProductId = product.Id;
        }
    }
}
