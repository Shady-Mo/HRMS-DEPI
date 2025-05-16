using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DEPI_Project.Repositories
{
    public interface IProjectRepository
    {
        List<Project> GetAllProjects();
        Project GetProjectById(int id);
        void AddProject(Project project, List<string> employeeIds);
        void AddAssignedEmployee(AssignedEmployees assignedEmployee);
        void UpdateProject(Project project);
        void DeleteProject(Project project);
        List<SelectListItem> GetProjectsByManagerId(string managerId);
        List<Project> GetProjectsByManager(string managerId);


    }
}