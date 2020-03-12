using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Models
{
    [Table("scheduled_shift")]
    public class ScheduledShift
    {
        [Key]
        public long Id { get; set; }

        [Column(TypeName ="Date"), Required]
        public DateTime Date { get; set; }

        [Column("shift_id"), Required]
        public long ShiftId { get; set; }

        [Required]
        public float Duration { get; set; }
    
        public virtual Shift Shift { get; set; }

        public ScheduledShift(long id, DateTime date, Shift shift)
        {
            Id = id;
            Date = date;
            Shift = shift;
        }

        public ScheduledShift()
        {
        }
    }
}
