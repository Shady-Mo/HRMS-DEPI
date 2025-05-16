using DEPI_Project.Models.CorpMgmt_System;

namespace DEPI_Project.Models.View_Model {
    public class JobViewModel {
        public string Name { get; set; }
        public JobStatus Status { get; set; }
        public string Role { get; set; }
        public DateTime ExpireDate { get; set; }
        public int Vacancies { get; set; }
        public int DepartmentId { get; set; }
        public JobType JobType { get; set; }
    }
}
