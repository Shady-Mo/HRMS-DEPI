using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;

namespace DEPI_Project.Repositories {
    public class HolidayRepository: IHolidayRepository {
        private readonly CorpMgmt_SystemContext context;

        public HolidayRepository(CorpMgmt_SystemContext context) {
            this.context = context;
        }

        public void AddHoliday(Holiday holiday) {
            context.Holidays.Add(holiday);
            context.SaveChanges();
        }

        public void DeleteHoliday(Holiday holiday) {
            context.Remove(holiday);
            context.SaveChanges();
        }

        public List<Holiday> GetAllHolidays() {
            return context.Holidays.ToList();
        }

        public Holiday GetHolidayById(int id) {
            return context.Holidays.FirstOrDefault(h => h.HolidayId == id);
        }

        public void UpdateHoliday(Holiday holiday) {
            context.Holidays.Update(holiday);
            context.SaveChanges();
        }
    }
}
