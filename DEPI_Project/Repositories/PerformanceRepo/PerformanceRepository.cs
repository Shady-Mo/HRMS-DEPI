using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using Microsoft.EntityFrameworkCore;

namespace DEPI_Project.Repositories {
    public class PerformanceRepository: IPerformanceRepository {
        private readonly CorpMgmt_SystemContext context;


        public PerformanceRepository(CorpMgmt_SystemContext context) {
            this.context = context;
        }

        public void AddPerformance(Performance performance) {
            context.Performances.Add(performance);
            context.SaveChanges();
        }

        public void DeletePerformance(int id) {
            context.Remove(id);
            context.SaveChanges();
        }

        public List<Performance> GetAllPerformances() {
            return context.Performances
                  .Include(p => p.Employee)
                  .Include(p => p.Task)
                  .Include(p => p.Manager)
                  .ToList();
        }

        public Performance GetPerformanceById(int id) {
            return context.Performances.FirstOrDefault(p => p.PerformanceId == id);
        }

        public void UpdatePerformance(Performance performance) {
            context.Update(performance);
            context.SaveChanges();
        }
        public async Task<List<object>> GetScoreDistributionDataAsync()
        {
            var scoreData = await context.Performances
                .GroupBy(p => p.Score)
                .Select(g => new {
                    ScoreName = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync();
            var orderedScoreData = scoreData
                .OrderBy(s => (int)Enum.Parse<Score>(s.ScoreName))
                .Cast<object>()
                .ToList();
            return orderedScoreData;
        }

        public async Task<List<object>> GetEmployeePerformanceOverTimeAsync(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
                return new List<object>();
            var performanceTimeline = await context.Performances
                .Where(p => p.EmployeeId == employeeId)
                .OrderBy(p => p.Date)
                .Select(p => new {
                    Date = p.Date.ToString("yyyy-MM-dd"),
                    ScoreValue = (int)p.Score
                })
                .Cast<object>()
                .ToListAsync();
            return performanceTimeline;
        }
    }
}
