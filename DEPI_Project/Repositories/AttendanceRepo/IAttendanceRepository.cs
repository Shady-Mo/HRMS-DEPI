using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPI_Project.Repositories.AttendanceRepo
{
    public interface IAttendanceRepository
    {
        List<Attendance> GetAllAttendances();
        Attendance GetAttendanceById(int id);
        void AddAttendance(Attendance attendance);
        void UpdateAttendance(Attendance attendance);
        void DeleteAttendance(Attendance attendance);
        List<SelectListItem> GetUsersAsSelectList();

    }
}
