using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models.CorpMgmt_System
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int EmployeeCapacity { get; set; }
        public int EstablishedYear { get; set; }
        [ForeignKey(nameof(Manager))]
        public string? ManagerId { get; set; }
        public Employee? Manager { get; set; }
        public List<Employee>? Employees { get; set; }

        public ICollection<Job>? Jobs { get; set; }

        [NotMapped]
        public int CurrentEmployeeCount => Employees?.Count ?? 0;


    }
}
