using DEPI_Project.Models.CorpMgmt_System.Context;
using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;

namespace DEPI_Project.Repositories.JobRepo
{
    public class JobRepository:IJobRepository
    {
        CorpMgmt_SystemContext context;
        public JobRepository()
        {
            context = new CorpMgmt_SystemContext();
        }
        public void Add(Job job)
        {
            context.Add(job);

        }
        public void Update(Job job)
        {
            context.Update(job);

        }
        public void Delete(Job job)
        {
            context.Remove(job);

        }
        public List<Job> GetAll()
        {

            return context.Jobs
                .Include(j => j.Department)
                .ToList();
        }
        public Job GetById(int id)
        {
            return context.Jobs
                   .Include(j => j.AppUser)
                   .FirstOrDefault(c => c.Id == id);

        }
        public void SaveInDB()
        {
            context.SaveChanges();
        }
        public List<SelectListItem> GetJobAsSelectList()
        {
            return context.Jobs
                .Include(j => j.AppUser)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
                .ToList();
        }
       

    }
}
