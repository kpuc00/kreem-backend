using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Models
{
    [Table("user_scheduled_shift")]
    public class UserScheduledShift
    {

        [Key]
        public long Id { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("scheduled_shift_id"), Required]
        public long ScheduledShiftId { get; set; }

        [Column("hourly_wage"), Required]
        public float HourlyWage { get; set; }

        public virtual User User { get; set; }

        public virtual ScheduledShift ScheduledShift { get; set; }

        public UserScheduledShift(User user, ScheduledShift scheduledShift)
        {
            User = user;
            ScheduledShift = scheduledShift;
            HourlyWage = user.HourlyWage;

        }

        public UserScheduledShift()
        {
        }
    }
}
