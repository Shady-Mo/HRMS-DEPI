using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEPI_Project.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly CorpMgmt_SystemContext context;
        private readonly UserManager<AppUser> dbContext;

        public ProjectRepository(CorpMgmt_SystemContext context, UserManager<AppUser> dbContext)
        {
            this.context = context;
            this.dbContext = dbContext;
        }

        public List<Project> GetAllProjects()
        {
            return context.Projects
                .Include(p => p.Manager)
                .Include(p => p.Department)
                .Include(p => p.AssignedEmployees)
                    .ThenInclude(pe => pe.Employee)
                .Include(p => p.Tasks)
                .ToList();
        }

        public Project GetProjectById(int id)
        {
            return context.Projects
                .Include(p => p.Manager)
                .Include(p => p.Department)
                .Include(p => p.AssignedEmployees)
                    .ThenInclude(pe => pe.Employee)
                .Include(p => p.Tasks)
                .FirstOrDefault(p => p.ProjectId == id);
        }

        public void AddProject(Project project, List<string> employeeIds)
        {
            // Initialize the assigned employees list if it is null
            if (project.AssignedEmployees == null)
            {
                project.AssignedEmployees = new List<AssignedEmployees>();
            }

            // Just use EmployeeId, EF will fill ProjectId automatically
            foreach (var employeeId in employeeIds)
            {
                project.AssignedEmployees.Add(new AssignedEmployees
                {
                    EmployeeId = employeeId
                });
            }

            try
            {
                context.Add(project); // EF will track the relationships
                context.SaveChanges(); // This will generate ProjectId and save assigned employees
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while adding the project.", ex);
            }
        }

        public void AddAssignedEmployee(AssignedEmployees assignedEmployee)
        {
            try
            {
                context.AssignedEmployees.Add(assignedEmployee);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while adding the assigned employee to the project.", ex);
            }
        }


        public void UpdateProject(Project project)
        {
            try
            {
                context.Update(project);
                context.SaveChanges();  
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while updating the project.", ex);
            }
        }

        public void DeleteProject(Project project)
        {
            try
            {
                context.Remove(project);
                context.SaveChanges();  
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while deleting the project.", ex);
            }
        }

        public List<SelectListItem> GetProjectsByManagerId(string managerId) {
            return context.Projects
                .Where(p => p.ManagerId == managerId) 
                .Select(p => new SelectListItem {
                    Value = p.ProjectId.ToString(),
                    Text = p.Name
                })
                .ToList();
        }

        public List<Project> GetProjectsByManager(string managerId) {
            var projects = context.Projects
                .Where(p => p.ManagerId == managerId || p.AssignedEmployees.Any(e=>e.EmployeeId==managerId))
                .Include(p => p.AssignedEmployees)
                    .ThenInclude(pe => pe.Employee)
                .Include(p => p.Department)
                .ToList();
            return projects;
        }

        public List<Project> GetAllProjectsWithEmployees()
        {
            return context.Projects
                .Include(p => p.AssignedEmployees)
                    .ThenInclude(pe => pe.Employee)
                .ToList();
        }
    }
}
