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

        [Column("type"), Required]
        public string TypeStr { get; set; }

        [Column("user_id"), Required]
        public long UserId { get; set; }

        [Column("request_id"), Required]
        public long RequestId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("date"), Required]
        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public RestockStageType? Type
        {
            get => string.IsNullOrEmpty(TypeStr) ? null : Enum.Parse(typeof(RestockStageType), TypeStr) as RestockStageType?;
            set => TypeStr = value.ToString();
        }

        public virtual User User { get; set; }

        public RestockRequest Request { get; set; }


    }

}
