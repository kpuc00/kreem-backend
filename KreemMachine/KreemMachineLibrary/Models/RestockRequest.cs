using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Models
{
    [Table("restock_request")]
    public class RestockRequest
    {

        [Key]
        public long Id { get; set; }

        [Column("product_id"), Required]
        public long ProductId { get; set; }

        public virtual Product Product { get; set; }

        public RestockRequest(){}

        public RestockRequest(Product product)
        {
            ProductId = product.Id;
        }
    }
}
