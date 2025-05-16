using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPI_Project.Repositories.CandidateRepo
{
    public interface ICandidateRepository
    {
        public void Add(Candidate candidate);
        public void Update(Candidate candidate);
        public void Delete(Candidate candidate);
        public List<Candidate> GetAll();
        public Candidate GetById(int id);
        public void SaveInDB();
        public List<SelectListItem> GetJobList();


    }
}
