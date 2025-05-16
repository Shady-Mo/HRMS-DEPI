using DEPI_Project.Models.CorpMgmt_System;

namespace DEPI_Project.Models.View_Model {
    public class PerformanceViewModel {
        public string EmployeeId { get; set; }
        public int TaskId { get; set; }
        public Score Score { get; set; }
    }
}
