using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models.CorpMgmt_System
{
    public enum LeaveStatus
    {
        Approved,
        Rejected,
        Pending

    }
    public enum LeaveType
    {
        Sick,
        Emergency,
        Annual,

    }
    public class Leave
    {
        #region Constructor
        public Leave()
        {
            Status = LeaveStatus.Pending;
            RequestDate = DateTime.Now;
        }
        #endregion
        #region Properties
        public int LeaveId { get; set; }
        [Required(ErrorMessage = "You Must Enter Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "You Must Enter End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ApprovalDate { get; set; }
        [Required(ErrorMessage = "You Must Enter The Reason For Your Leave")]
        [MinLength(2, ErrorMessage = "Reason Must Be More Than 2 Characters")]
        [MaxLength(200, ErrorMessage = "Reason Must Be Less Than 200 Characters")]
        public string Reason { get; set; }
        public LeaveStatus Status { get; set; }
        public LeaveType Type { get; set; }

        public string? EmpId { get; set; }
        public Employee? Employee { get; set; }

        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        #endregion

        #region Methods
        public int TotalLeaveDays()
        {
            return (EndDate - StartDate).Days + 1;
        }

        #endregion

    }
}
