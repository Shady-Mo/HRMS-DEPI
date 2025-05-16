using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models.CorpMgmt_System
{
    public enum MaritalStatus
    {
        Single,
        Married,
        Divorced,
        Widowed
    }

    public enum ContractType
    {
        FullTime,
        PartTime,
        Internship
    }

    public enum StatusType
    {
        Active,
        [Display(Name = "In Active")]
        Inactive,
        [Display(Name = "On Leave")]
        OnLeave
    }

    public class Employee: AppUser
    {
        public decimal Salary { get; set; }
        public string Position { get; set; }
        public StatusType EmpStatus { get; set; }
        public ContractType ContractType { get; set; }
        public DateTime HireDate { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public string[] Skills { get; set; }
        public string? StripeAccountId { get; set; }
        [ForeignKey(nameof(Department))]
        public int DeptId { get; set; }
        public Department? Department { get; set; }
        public List<Attendance>? Attendances { get; set; }
        public List<Leave>? Leaves { get; set; }
        public List<ShiftSchedule>? ShiftSchedules { get; set; }
        public List<Payment>? Payments { get; set; }
        public Department? ManagedDepartment { get; set; }
        public List<ProjectTask>? AssignedTasks { get; set; } // employee
        public List<ProjectTask>? ManagedTasks { get; set; } // manager
        public List<Project>? Projects { get; set; }
        public List<Performance>? Performances { get; set; } // Employee 
        public List<Performance>? AssignedPerformances { get; set; } // Manager
        public List<AssignedEmployees> AssignedEmployees { get; set; }
        //public List<ProjectEmployee> ProjectEmployees { get; set; }



        public string? ManagerId { get; set; }
        [ForeignKey(nameof(ManagerId))]
        public Employee? Manager { get; set; }





    }
}
