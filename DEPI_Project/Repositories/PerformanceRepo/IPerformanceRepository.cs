using DEPI_Project.Models.CorpMgmt_System;

namespace DEPI_Project.Repositories {
    public interface IPerformanceRepository {
        List<Performance> GetAllPerformances();
        Performance GetPerformanceById(int id);
        void AddPerformance(Performance performance);
        void UpdatePerformance(Performance performance);
        void DeletePerformance(int id);
        Task<List<object>> GetScoreDistributionDataAsync();
        Task<List<object>> GetEmployeePerformanceOverTimeAsync(string employeeId);
    }
}
