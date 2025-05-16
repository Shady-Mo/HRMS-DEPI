using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPI_Project.Repositories.JobRepo
{
    public interface IJobRepository
    {
        public void Add(Job job);
        public void Update(Job job);
        public void Delete(Job job);
        public List<Job> GetAll();
        public Job GetById(int id);
        public void SaveInDB();
        List<SelectListItem> GetJobAsSelectList();
        
    }
}
