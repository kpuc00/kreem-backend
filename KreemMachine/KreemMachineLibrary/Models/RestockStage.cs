using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Models.Statics
{
    [Table("restock_stage")]
    public class RestockStage
    {
        [Key]
        public long Id { get; set; }

        [Column("status"), Required]
        public string StatusStr { get; set; }

        [Column("user_id"), Required]
        public long UserId { get; set; }

        [Column("restock_id"), Required]
        public long RestockId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("date"), Required]
        public DateTime Date { get; set; }

        public virtual User User { get; set; }

        public RestockRequest Request { get; set; }


    }

}
