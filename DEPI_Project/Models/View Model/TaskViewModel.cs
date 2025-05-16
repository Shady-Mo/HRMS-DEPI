using DEPI_Project.Models.CorpMgmt_System;

namespace DEPI_Project.Models.View_Model
{
    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Blocked
    }
    public class TaskViewModel
    {
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
        public string EmployeeId { get; set; }
        public int ProjectId { get; set; }
    }
}