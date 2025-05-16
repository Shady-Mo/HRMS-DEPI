using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DEPI_Project.Repositories
{
    public interface IDepartmentRepository
    {
        List<Department> GetAllDepartments();
        Department GetDepartmentById(int id);
        void AddDepartment(Department department);
        void UpdateDepartment(Department department);
        void DeleteDepartment(Department department);
        Department GetByName(string name);
        List<SelectListItem> GetDepartmentsAsSelectList();
    }
}