using DEPI_Project.Models.CorpMgmt_System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DEPI_Project.Repositories {
    public interface IEmployeeRepository
    {
        bool CheckEmployeeByEmail(string email);
        bool CheckEmployeeByUserName(string userName);
        Task<string> CheckEmployeeLogin(string email, string password);
        Task<IdentityResult> AddPassword(Employee employee, string password);
        public List<Employee> GetAll();
        List<Employee> GetAdmins();
        List<SelectListItem> GetAdminList();
        List<Employee> GetManagers();
        public Employee GetById(string id);

        public void Add(Employee employee);

        public void Update(Employee employee);

        public void Delete(Employee employee);

        public void SaveInDB();
        List<SelectListItem> GetUsersAsSelectList();
        //************* Emp dashboard *************
        // Attendace
        List<Attendance> GetMyAttendances(string userId);
        Attendance? GetTodayAttendance(string userId);
        void AddAttendance(Attendance attendance);
        void UpdateAttendance(Attendance attendance);
        // Profile
        Employee? GetEmployeeProfile(string userId);

        // Leave
        List<Leave> GetMyLeaves(string userId);
        void SubmitLeave(Leave leave);
        void CancelLeave(int leaveId, string userId);
        List<SelectListItem> GetEmployeesAsSelectList();

    }

}
