using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using Microsoft.EntityFrameworkCore;

namespace DEPI_Project.Repositories {
    public class LeaveRepository: ILeaveRepository {
        private readonly CorpMgmt_SystemContext context;

        public LeaveRepository(CorpMgmt_SystemContext context) {
            this.context = context;
        }

        public List<Leave> GetAllLeaves()
        {
            return context.Leaves
                .Include(l => l.Employee)
                .ToList();
        }

        public List<Leave> GetPendingLeaves()
        {
            return context.Leaves
                .Include(l => l.Employee)
                .Where(l => l.Status == LeaveStatus.Pending)
                .ToList();
        }

        public Leave GetLeaveById(int id)
        {
            return context.Leaves
                .Include(l => l.Employee)
                .FirstOrDefault(l => l.LeaveId == id);
        }

        public void AddLeave(Leave leave) {
            context.Add(leave);
            context.SaveChanges();
        }

        public void DeleteLeave(Leave leave) {
            context.Remove(leave);
            context.SaveChanges();
        }

        public void UpdateLeave(Leave leave) {
            context.Update(leave);
            context.SaveChanges();
        }

        //For Admin 
        public void ApproveLeave(int id)
        {
            var leave = GetLeaveById(id);
            if (leave != null)
            {
                leave.Status = LeaveStatus.Approved;
                leave.ApprovalDate = DateTime.Now;
                context.SaveChanges();
            }
        }

        public void RejectLeave(int id)
        {
            var leave = GetLeaveById(id);
            if (leave != null)
            {
                leave.Status = LeaveStatus.Rejected;
                leave.ApprovalDate = DateTime.Now;
                context.SaveChanges();
            }
        }



    }
}
