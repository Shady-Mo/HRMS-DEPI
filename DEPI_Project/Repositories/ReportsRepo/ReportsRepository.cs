using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using DEPI_Project.Repositories.AttendanceRepo;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DEPI_Project.Repositories.ReportsRepo
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly CorpMgmt_SystemContext context;
        IAttendanceRepository attendanceRepository;
        ILeaveRepository leaveRepository;
        IHolidayRepository holidayRepository;
        public ReportsRepository(CorpMgmt_SystemContext _context , IAttendanceRepository _attendanceRepository ,
            ILeaveRepository _leaveRepository , IHolidayRepository _holidayRepository)
        {
            context = _context;
            attendanceRepository = _attendanceRepository;
            leaveRepository = _leaveRepository;
            holidayRepository = _holidayRepository;
        }

        public List<Attendance> GetAttendanceReport(string? employeeName, DateTime? date)
        {
            var attendances = attendanceRepository.GetAllAttendances();
            if (employeeName != null)
            {
                attendances = attendances
                    .Where(a => a.Employee.FirstName.ToLower().Contains(employeeName))
                    .ToList();
            }

            if (date.HasValue)
            {
                attendances = attendances
                   .Where(a => a.Date.Date == date.Value.Date)
                   .ToList();
            }

            return attendances;
        }

        public List<Holiday> GetHolidayReport(int? year)
        {
            var holidays = holidayRepository.GetAllHolidays();

            if (year.HasValue)
            {
                holidays = holidays.Where(h => h.StartDate.Year == year)
                    .ToList();
            }
                

            return holidays;
        }


        public List<Leave> GetLeaveReport(string? employeeName, DateTime? startDate, DateTime? endDate, LeaveStatus? status)
        {
            var leaves = leaveRepository.GetAllLeaves();

            if (employeeName != null)
            {
                leaves=  leaves
                    .Where(l => l.Employee.FirstName.ToLower().Contains(employeeName))
                    .ToList();
            }

            if (startDate.HasValue)
            {
                leaves = leaves
                   .Where(l => l.StartDate.Date == startDate.Value.Date)
                   .ToList();
            }

            if (endDate.HasValue)
            {
                leaves = leaves
                   .Where(l => l.EndDate.Date == endDate.Value.Date)
                   .ToList();
            }

            if (status.HasValue)
            {
                leaves = leaves
                    .Where(l => l.Status == status.Value)
                    .ToList();
            }

            return leaves;
        }

    }
}
