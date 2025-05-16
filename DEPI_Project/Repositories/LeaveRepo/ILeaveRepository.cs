using DEPI_Project.Models.CorpMgmt_System;

namespace DEPI_Project.Repositories {
    public interface ILeaveRepository {
        List<Leave> GetAllLeaves();
        public List<Leave> GetPendingLeaves();
        Leave GetLeaveById(int id);
        void AddLeave(Leave leave);
        void UpdateLeave(Leave leave);
        void DeleteLeave(Leave leave);
        public void ApproveLeave(int id);
        public void RejectLeave(int id);
    }
}
