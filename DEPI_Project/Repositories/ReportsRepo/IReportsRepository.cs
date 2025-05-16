using DEPI_Project.Models.CorpMgmt_System;

namespace DEPI_Project.Repositories.ReportsRepo
{
    public interface IReportsRepository
    {
        List<Attendance> GetAttendanceReport(string? employeeName, DateTime? date);
        List<Leave> GetLeaveReport(string? employeeName, DateTime? startDate, DateTime? endDate, LeaveStatus? status);
        List<Holiday> GetHolidayReport(int? year);

    }
}
