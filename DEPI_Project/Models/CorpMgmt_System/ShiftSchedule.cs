using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models.CorpMgmt_System
{
    public enum ShiftType
    {
        Day,
        Night
    }
    public enum ShiftStatus
    {
        Scheduled,
        Completed,
        Canceled,
        Pending
    }
    public class ShiftSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public TimeOnly BreakStart { get; set; }
        public TimeOnly BreakEnd { get; set; }

        public ShiftType? ShiftType { get; set; }
        public ShiftStatus? Status { get; set; }
        public DateOnly ShiftDate { get; set; }
        public string? Description { get; set; }

        //Method
        public double TotalHours()
        {
            return (EndTime - StartTime - (BreakEnd - BreakStart)).TotalHours;
        }


        [ForeignKey(nameof(Employee))]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [ForeignKey(nameof(Manager))]
        public string ManagerId { get; set; }
        public Employee Manager { get; set; }





    }
}
