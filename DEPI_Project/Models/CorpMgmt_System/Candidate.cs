using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models.CorpMgmt_System
{
    public class Candidate
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Mobile { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [ForeignKey(nameof(AppUser))] //admin 
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

        [ForeignKey(nameof(Job))]
        public int? JobId { get; set; }
        public Job? Job { get; set; }

    }
}
