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
        public double Duration { get; set; }
    
        public virtual Shift Shift { get; set; }

        public virtual ICollection<UserScheduledShift> EmployeeScheduledShits { get; set; }

        public bool isUnderstaffed => EmployeeScheduledShits == null || EmployeeScheduledShits.Count < Shift.MinStaff;

        public bool IsOverstaffed => EmployeeScheduledShits?.Count > Shift.MaxStaff;

        public ScheduledShift(DateTime date, Shift shift)
        {
            Date = date;
            ShiftId = shift.Id;
            //Shift = shift;
            Duration = shift.Duration;
        }

        public ScheduledShift()
        {
        }
    }
}
