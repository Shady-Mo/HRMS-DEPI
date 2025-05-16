using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DEPI_Project.Repositories.CandidateRepo
{
    public class CandidateRepository:ICandidateRepository
    {
        CorpMgmt_SystemContext context;
        public CandidateRepository()
        {
            context = new CorpMgmt_SystemContext();
        }
        public void Add(Candidate candidate)
        {
            context.Add(candidate);
        }
        public void Update(Candidate candidate)
        {
            context.Update(candidate);
        }
        public void Delete(Candidate candidate)
        {
            context.Remove(candidate);
        }
        public List<Candidate> GetAll()
        {
            return context.Candidates
                .Include(c => c.Job)
                .ToList();
        }
        public Candidate GetById(int id)
        {
            return context.Candidates
                   .Include(c => c.AppUser)
                   .Include(c => c.Job)
                   .FirstOrDefault(c => c.Id == id);
        }
        public void SaveInDB()
        {
            context.SaveChanges();
        }
        public List<SelectListItem> GetJobList()
        {
            return context.Jobs
            .Select(j => new SelectListItem
            {
                Value = j.Id.ToString(),
                Text = j.Name
            })
           .ToList();

        }

    }
}
