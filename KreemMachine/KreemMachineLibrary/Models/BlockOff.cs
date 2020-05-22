using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Models
{
    [Table("block_off")]
    public class BlockOff
    {

        [Key]
        public long Id { get; set; }

        [Column("user_id"), Required]
        public long UserId { get; set; }

        [Column("scheduled_shift_id"), Required]
        public long ScheduledShiftId { get; set; }

        [Column("date"), Required]
        public DateTime Date { get; set; }

        public virtual User User { get; set; }

        public virtual ScheduledShift ScheduledShift { get; set; }

    }
}
