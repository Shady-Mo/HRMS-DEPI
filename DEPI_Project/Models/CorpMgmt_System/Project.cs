using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models.CorpMgmt_System
{
    public enum ProjectStatus
    {
        Active,
        Completed,
        OnHold,
        Canceled
    }

    public enum PriorityLevel
    {
        Low,
        Medium,
        High,
    }
    public class Project
    {

        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ProjectStatus Status { get; set; }
        public decimal Budget { get; set; }
        public PriorityLevel Priority { get; set; }
        public DateTime DueDate { get; set; }

        [ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public List<ProjectTask>? Tasks { get; set; }
        public List<AssignedEmployees> AssignedEmployees { get; set; }
        //public List<AssignedEmployees> ProjectEmployees { get; set; }



        //Method
        public double CalculateProgressPercentage()
        {
            int totalTasks = Tasks?.Count ?? 0;
            int completedTasks = Tasks?.Count(t => t.Status == TaskStatus.Completed) ?? 0;

            if (totalTasks == 0)
                return 0;

            return (double)(completedTasks) / totalTasks * 100;

        }



        [ForeignKey(nameof(Manager))]
        public string ManagerId { get; set; }
        public Employee? Manager { get; set; }
    }
}
