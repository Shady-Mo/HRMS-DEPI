namespace DEPI_Project.Models.CorpMgmt_System
{
    public class AssignedEmployees
    {
        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public string EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
