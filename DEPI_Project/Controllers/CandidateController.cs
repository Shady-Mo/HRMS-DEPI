using System.Data;
using ClosedXML.Excel;
using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using DEPI_Project.Repositories.CandidateRepo;
using DEPI_Project.Repositories.JobRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DEPI_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CandidateController : Controller
    {
        ICandidateRepository _candidateRepository;
        IJobRepository _jobRepository;
        public CandidateController(IJobRepository jobRepository,ICandidateRepository candidateRepository)
        {
            _candidateRepository = candidateRepository;
            _jobRepository = jobRepository;
        }

        #region GET All
        public ActionResult Index()
        {
            ViewData["JobList"] =_jobRepository.GetAll();
            List<Candidate> CandidateModel = _candidateRepository.GetAll();
            return View(CandidateModel);
        }
        #endregion

        #region Details
        public ActionResult Details(int id)
        {
            ViewData["JobList"] = _jobRepository.GetJobAsSelectList();
            Candidate candidate = _candidateRepository.GetById(id);
            return View();
        }
        #endregion

        #region Get job Json data 
        [HttpGet]
        public JsonResult GetJobDetails(int jobId)
        {
            var job = _jobRepository.GetById(jobId);
            if (job == null)
                return Json(null);
            string appUserFullName = job.AppUser.FirstName + " " + job.AppUser.LastName;

            return Json(new
            {
                jobType = job.JobType.ToString(),
                role = job.Role,
                appUserId = job.AppUserId,
                appUserName = appUserFullName
            });
        }
        #endregion

        #region Add
        public ActionResult Add()
        {
            ViewData["JobList"] =_jobRepository.GetJobAsSelectList();
            return View();

        }
        #endregion

        #region SaveAdd
        [HttpPost]
        public IActionResult SaveAdd(Candidate CandidateFromReq)
        {
            if (ModelState.IsValid)
            {
               _candidateRepository.Add(CandidateFromReq);
               _candidateRepository.SaveInDB();
                TempData["Success"] = "Candidate saved successfully";
                return RedirectToAction(nameof(Index));
            }

            ViewData["JobList"] = _jobRepository.GetJobAsSelectList();
            return View("Add", CandidateFromReq);

        }
        #endregion

        #region Edit
        public ActionResult Edit(int id)
        {
            ViewData["JobList"] = _jobRepository.GetJobAsSelectList();
            Candidate candidate = _candidateRepository.GetById(id);
            return View(candidate);
        }
        #endregion

        #region SaveEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Candidate candidateFromReq, [FromRoute] int id)
        {
            if (id != candidateFromReq.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                Candidate candidateFromDB = _candidateRepository.GetById(id);
               
                    candidateFromDB.Name = candidateFromReq.Name;
                    candidateFromDB.Email = candidateFromReq.Email;
                    candidateFromDB.Mobile = candidateFromReq.Mobile;
                    candidateFromDB.JobId = candidateFromReq.JobId;
                    candidateFromDB.AppUserId = candidateFromReq.AppUserId;
                    _candidateRepository.Update(candidateFromDB);
                    _candidateRepository.SaveInDB();
                TempData["EditSuccess"] = "Candidate updated successfully";
                return RedirectToAction(nameof(Index));
            }
            ViewData["JobList"] = _jobRepository.GetJobAsSelectList();
            return View("Edit", candidateFromReq);
        }
        #endregion

        #region Delete
        public IActionResult Delete(int id)
        {
            Candidate candidate = _candidateRepository.GetById(id);
            if (ModelState.IsValid)
            {
                _candidateRepository.Delete(candidate);
                _candidateRepository.SaveInDB();
                TempData["DeleteSuccess"] = "Candidate deleted successfully";
                return RedirectToAction("Index");
            }
            return NotFound();

        }
        #endregion

        #region Export to Excel
        public IActionResult ExportToExcel()
        {
            var candidates = _candidateRepository.GetAll() // غيريها حسب مصدر البيانات
                .Select(c => new
                {
                    FullName = c.Name,
                    JobTitle = c.Job?.Name ?? "N/A",
                    Mobile = c.Mobile,
                    JobType = c.Job?.JobType.ToString() ?? "N/A",
                    Role = c.Job?.Role ?? "N/A",
                    Email = c.Email
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Candidates");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "Full Name";
                worksheet.Cell(currentRow, 2).Value = "Job Title";
                worksheet.Cell(currentRow, 3).Value = "Mobile";
                worksheet.Cell(currentRow, 4).Value = "Job Type";
                worksheet.Cell(currentRow, 5).Value = "Role";
                worksheet.Cell(currentRow, 6).Value = "Email";

                // Data
                foreach (var c in candidates)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = c.FullName;
                    worksheet.Cell(currentRow, 2).Value = c.JobTitle;
                    worksheet.Cell(currentRow, 3).Value = c.Mobile;
                    worksheet.Cell(currentRow, 4).Value = c.JobType;
                    worksheet.Cell(currentRow, 5).Value = c.Role;
                    worksheet.Cell(currentRow, 6).Value = c.Email;
                }

                // Header Style
                var headerRange = worksheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Candidates-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                }
            }
        }

        #endregion

    }
}
