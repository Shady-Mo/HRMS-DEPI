using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using Microsoft.EntityFrameworkCore;

namespace DEPI_Project.Repositories
{
    public class ShiftScheduleRepository : IShiftScheduleRepository
    {
        private readonly CorpMgmt_SystemContext context;

        public ShiftScheduleRepository(CorpMgmt_SystemContext context)
        {
            this.context = context;
        }

        public List<ShiftSchedule> GetAllShifts()
        {
            return context.ShiftSchedules
                .Include(s => s.Employee)
                .Include(s => s.Manager)
                .ToList();
        }

        public ShiftSchedule GetShiftById(int id)
        {
            return context.ShiftSchedules
                .Include(s => s.Employee)
                .Include(s => s.Manager)
                .FirstOrDefault(s => s.ScheduleId == id);
        }

        public void AddShift(ShiftSchedule shift)
        {
            context.Add(shift);
            context.SaveChanges();
        }

        public void UpdateShift(ShiftSchedule shift)
        {
            context.Update(shift);
            context.SaveChanges();
        }

        public void DeleteShift(ShiftSchedule shift)
        {
            context.Remove(shift);
            context.SaveChanges();
        }

        public void MarkShiftCompleted(int id)
        {
            var shift = GetShiftById(id);
            if (shift != null)
            {
                shift.Status = ShiftStatus.Completed;
                context.SaveChanges();
            }
        }

        public void CancelShift(int id)
        {
            var shift = GetShiftById(id);
            if (shift != null)
            {
                shift.Status = ShiftStatus.Canceled;
                context.SaveChanges();
            }
        }
    }
}