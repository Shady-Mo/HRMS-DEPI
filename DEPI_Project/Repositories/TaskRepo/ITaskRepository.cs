using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPI_Project.Repositories
{
    public interface ITaskRepository
    {
        List<ProjectTask> GetAllTasks();
        List<ProjectTask> GetTasksByManagerId(string managerId);
        ProjectTask GetTaskById(int id);
        void AddTask(ProjectTask task);
        void UpdateTask(ProjectTask task);
        void DeleteTask(int id);
        public List<SelectListItem> GetListTasks();
        List<ProjectTask> GetTasks(string EmpId);
    }
}