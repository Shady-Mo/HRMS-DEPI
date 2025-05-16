using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models.CorpMgmt_System
{
    public enum Status
    {
        Present,
        Absent
    }
    public class Attendance
    {
        public int AttendanceId { get; set; }
        [Required(ErrorMessage = "You Must Enter Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "You Must Enter The Entry Time")]
        [DataType(DataType.Time)]
        public TimeSpan TimeIn { get; set; }
        [Required(ErrorMessage = "You Must Enter The Checkout Time")]
        [DataType(DataType.Time)]
        public TimeSpan TimeOut { get; set; }
        public Status Status { get; set; }

        public string EmpId { get; set; }
        public Employee? Employee { get; set; }

        public string AppUserId { get; set; }
        public AppUser? AppUser{ get; set; }

        //method
        //public TimeSpan TotalHours()
        //{
        //    return (TimeOut - TimeIn);
        //}
        public TimeSpan TotalHours()
        {
            var startDateTime = Date.Date + TimeIn;
            var endDateTime = Date.Date + TimeOut;

            if (endDateTime < startDateTime)
            {
                endDateTime = endDateTime.AddDays(1);
            }

            return endDateTime - startDateTime;
        }


    }
}
