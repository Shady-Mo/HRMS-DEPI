using DEPI_Project.Models.CorpMgmt_System;

namespace DEPI_Project.Repositories {
    public interface IHolidayRepository {
        List<Holiday> GetAllHolidays();
        Holiday GetHolidayById(int id);
        void AddHoliday(Holiday holiday);
        void UpdateHoliday(Holiday holiday);
        void DeleteHoliday(Holiday holiday);
    }
}
