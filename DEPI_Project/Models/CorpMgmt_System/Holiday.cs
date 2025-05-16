using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models.CorpMgmt_System
{
    public class Holiday
    {
        public int HolidayId { get; set; }
        [Display(Name = "Holiday Name")]
        [Required(ErrorMessage = "You Must Enter Holiday Name")]
        [StringLength(100, ErrorMessage = "Holiday Name Must Not Exceed 100 Characters")]
        public string HolidayName { get; set; }
        [Required(ErrorMessage = "You Must Enter Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "You Must Enter End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [ForeignKey(nameof(AppUser))]
        public string UserId { get; set; }
        public AppUser? AppUser { get; set; }

        // method
        public int HolidayDuration => (EndDate - StartDate).Days + 1;

    }
}
