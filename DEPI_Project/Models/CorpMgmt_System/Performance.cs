using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models.CorpMgmt_System
{
    public class Performance
    {
        public int PerformanceId { get; set; }
        public Score Score { get; set; } = Score.None;
        public string Review { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }

        // Navigation properties
        [ForeignKey(nameof(Employee))]
        public string EmployeeId { get; set; }
        public virtual Employee ?Employee { get; set; }

        [ForeignKey(nameof(Task))]
        public int TaskId { get; set; }
        public virtual ProjectTask ?Task { get; set; }

        public string ManagerId { get; set; }
        public Employee ?Manager { get; set; }

        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }


    }

    public enum Score {
        None,
        VeryPoor,
        Poor,
        Average,
        Good,
        VeryGood,
    }
}
