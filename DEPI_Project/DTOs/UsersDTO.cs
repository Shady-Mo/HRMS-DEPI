using DEPI_Project.Models.CorpMgmt_System;

namespace DEPI_Project.DTOs
{
    public class UsersDTO
    {
        public string? UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class EmployeeDTO : UsersDTO
    {
        public decimal Salary { get; set; }
        public string Position { get; set; }
        public StatusType EmpStatus { get; set; }
        public ContractType ContractType { get; set; }
        public DateTime HireDate { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public int DeptId { get; set; }
        public string[] Skills { get; set; }

    }
}