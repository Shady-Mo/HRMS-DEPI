using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models.CorpMgmt_System
{
    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Blocked
    }
    public class ProjectTask
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; }
        public PriorityLevel Priority { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public DateTime CompletedDate { get; set; }



        //Method
        public double ProgressPercentage()
        {
            if (EstimatedHours == 0)
                return 0;
            return (double)(ActualHours) / EstimatedHours * 100;
        }


        // Navigation properties
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        //    public virtual Manager Manager { get; set; } 
        [ForeignKey(nameof(Employee))]
        public string EmployeeId { get; set; }
        public Employee? Employee { get; set; }


        [ForeignKey(nameof(Manager))]
        public string ManagerId { get; set; }
        public Employee ?Manager { get; set; }

        //
        public Performance? Performance { get; set; }



    }
}
