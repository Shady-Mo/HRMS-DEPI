using DEPI_Project.Models.CorpMgmt_System.Context;
using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPI_Project.Repositories.AttendanceRepo
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly CorpMgmt_SystemContext context;

        public AttendanceRepository(CorpMgmt_SystemContext context)
        {
            this.context = context;
        }

        public List<Attendance> GetAllAttendances()
        {
            return context.Attendances
                  .Include(a => a.Employee)
                  .Include(a => a.AppUser)
                  .ToList();
        }

        public Attendance GetAttendanceById(int id)
        {
            return context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefault(A => A.AttendanceId == id);
        }

        public void AddAttendance(Attendance attendance)
        {
            context.Attendances.Add(attendance);
            context.SaveChanges();
        }

        public void UpdateAttendance(Attendance attendance)
        {
            context.Attendances.Update(attendance);
            context.SaveChanges();
        }

        public void DeleteAttendance(Attendance attendance)
        {
            context.Remove(attendance);
            context.SaveChanges();
        }

        public List<SelectListItem> GetUsersAsSelectList()
        {
            return context.Users
                .Where(u => u.Type == "Employee")
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.UserName
                })
                .ToList();
        }

        
    }
}
