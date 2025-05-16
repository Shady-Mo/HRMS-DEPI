using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DEPI_Project.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly CorpMgmt_SystemContext context;
        private readonly UserManager<AppUser> dbContext;

        public TaskRepository(CorpMgmt_SystemContext context, UserManager<AppUser> dbContext)
        {
            this.context = context;
            this.dbContext = dbContext;
        }

        public void AddTask(ProjectTask task)
        {
            context.ProjectTasks.Add(task);
            context.SaveChanges();
        }

        public void DeleteTask(int id)
        {
            context.ProjectTasks.Remove(GetTaskById(id));
            context.SaveChanges();
        }

        public List<ProjectTask> GetAllTasks()
        {
            return context.ProjectTasks.ToList();
        }
        public List<SelectListItem> GetListTasks()
        {
            return context.ProjectTasks
                .Select(t => new SelectListItem
                {
                    Value = t.TaskId.ToString(),
                    Text = t.Title
                }).ToList();
        }


        public List<ProjectTask> GetTasksByManagerId(string managerId)
        {
            var tasks = context.ProjectTasks
                .Where(pt => pt.ManagerId == managerId)
                .Include(pt => pt.Project)
                .Include(pt => pt.Manager)
                .Include(pt => pt.Employee)
                .ToList();
            return tasks;
        }
        public List<ProjectTask> GetTasks(string EmpId)
        {
            var task = context.ProjectTasks
                .Where(p=>p.EmployeeId==EmpId)
                .Include(p => p.Project)
                .ToList();
            return task;
        }

        public ProjectTask GetTaskById(int id)
        {
            return context.ProjectTasks.FirstOrDefault(t => t.TaskId == id);
        }

        public void UpdateTask(ProjectTask task)
        {
            context.ProjectTasks.Update(task);
            context.SaveChanges();
        }
    }
}