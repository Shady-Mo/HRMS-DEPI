using DEPI_Project.Models.CorpMgmt_System;
using DEPI_Project.Models.CorpMgmt_System.Context;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DEPI_Project.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly CorpMgmt_SystemContext _context;

        public DepartmentRepository(CorpMgmt_SystemContext context)
        {
            _context = context;
        }

        public List<Department> GetAllDepartments()
        {
            return _context.Departments
                .Include(d => d.Manager)
                .ToList();
        }

        public Department GetDepartmentById(int id)
        {
            return _context.Departments
                .Include(d => d.Manager)
                .FirstOrDefault(d => d.DepartmentId == id);
        }

        public void AddDepartment(Department department)
        {
            _context.Departments.Add(department);
            _context.SaveChanges();
        }

        public void UpdateDepartment(Department department)
        {
            _context.Departments.Update(department);
            _context.SaveChanges();
        }

        public void DeleteDepartment(Department department)
        {
            _context.Departments.Remove(department);
            _context.SaveChanges();
        }
        public List<SelectListItem> GetDepartmentsAsSelectList()
        {
            return _context.Departments
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(), 
                    Text = d.Name
                })
                .ToList();
        }

        public Department GetByName(string name) {
            return _context.Departments
                .FirstOrDefault(d => d.Name == name);
        }
    }
}