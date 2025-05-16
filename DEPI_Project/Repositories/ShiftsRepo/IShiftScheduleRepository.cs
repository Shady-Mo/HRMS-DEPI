using DEPI_Project.Models.CorpMgmt_System;

namespace DEPI_Project.Repositories
{
    public interface IShiftScheduleRepository
    {
        List<ShiftSchedule> GetAllShifts();
        ShiftSchedule GetShiftById(int id);
        void AddShift(ShiftSchedule shift);
        void UpdateShift(ShiftSchedule shift);
        void DeleteShift(ShiftSchedule shift);
        void MarkShiftCompleted(int id);
        void CancelShift(int id);
    }
}