using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Models
{
    /// <summary>
    /// A user of the system as stored in the DB
    /// </summary>
    [Table("shift")]
    public class Shift
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Column("start_hour"), Required]
        public TimeSpan StartHour { get; set; }

        [Column("end_hour"), Required]
        public TimeSpan EndHour { get; set; }

        [Column("min_staff"), Required]
        public int MinStaff { get; set; }

        [Column("max_staff"), Required]
        public int MaxStaff { get; set; }

        [Column("preferred_staff"), Required]
        public int PreferredStaff { get; set; }

        [NotMapped]
        public double Duration => (StartHour > EndHour
                                    ? EndHour.Add(new TimeSpan(1, 0, 0, 0, 0)) - StartHour
                                    : StartHour - EndHour).Duration().TotalHours;

        internal Shift()
        {
        }

        internal Shift(long id, string name, TimeSpan startHour, TimeSpan endHour, int minStaff, int maxStaff, int preferredStaff)
        {
            Id = id;
            Name = name;
            StartHour = startHour;
            EndHour = endHour;
            MinStaff = minStaff;
            MaxStaff = maxStaff;
            PreferredStaff = preferredStaff;
        }


    }
}
