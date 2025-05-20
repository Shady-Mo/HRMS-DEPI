using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPI_Project.Models.View_Model {
    // Create this in your Models folder (e.g., DEPI_Project.Models.CorpMgmt_System.ViewModels)
    public class AddProjectViewModel {
        public Project Project { get; set; }
        public SelectList ManagerList { get; set; }
        public SelectList DepartmentList { get; set; }
        public List<Employee> EmployeeList { get; set; }
    }
}
