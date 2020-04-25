using KreemMachineLibrary.Models.Statics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Models
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

        [Column("request_id"), Required]
        public long RequestId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("date"), Required]
        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public RestockStageType? Status
        {
            get => string.IsNullOrEmpty(StatusStr) ? null : Enum.Parse(typeof(RestockStageType), StatusStr) as RestockStageType?;
            set => StatusStr = value.ToString();
        }

        public virtual User User { get; set; }

        public RestockRequest Request { get; set; }


    }

}
