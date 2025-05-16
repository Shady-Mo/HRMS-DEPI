using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models.CorpMgmt_System
{
    public enum JobStatus
    {
        Open,
        Closed,
        OnHold
    }

    public enum JobType
    {
        FullTime,
        PartTime
       
    }
   
    public class Job
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public JobStatus Status { get; set; }

        public string Role { get; set; }

        public int Vacancies { get; set; }
        public DateTime ExpireDate { get; set; }

        public JobType JobType { get; set; }

        [ForeignKey(nameof(AppUser))]  //admin
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }


        [ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }


        public ICollection<Candidate>? Candidates { get; set; }

    }
}
