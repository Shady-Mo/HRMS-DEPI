using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models.CorpMgmt_System
{
    public class AppUser : IdentityUser
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string Nationality { get; set; }

        public string Type { get; set; }

        public string Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public ICollection<Performance>? Performances { get; set; }

        public ICollection<Holiday>? Holidays { get; set; }

        public ICollection<Leave>? Leaves { get; set; }

        public ICollection<Attendance>? Attendances { get; set; }

        public ICollection<Payment>? Payments { get; set; }

        public ICollection<Job>? Jobs { get; set; }
        public ICollection<Candidate>? Candidates { get; set; }

    }
    
    public enum Gender
    {
        Male,
        Female
    }
}
