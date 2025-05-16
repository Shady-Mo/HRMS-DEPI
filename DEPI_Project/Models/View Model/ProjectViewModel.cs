using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPI_Project.Models.View_Model {
    public class ProjectViewModel {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public ProjectStatus Status { get; set; }
        public decimal Budget { get; set; }
        public PriorityLevel Priority { get; set; }
        public DateTime DueDate { get; set; }
        public int DepartmentId { get; set; }
        public string ManagerId { get; set; }
        public List<string> ProjectEmployees { get; set; } = new List<string>();
    }
}
