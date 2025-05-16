using System.ComponentModel.DataAnnotations.Schema;

namespace DEPI_Project.Models.CorpMgmt_System
{
    public enum PaymentStaus
    {
        Paid,
        Pending
    }

    public class Payment
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public decimal Bonuses { get; set; }
        public decimal Deductions { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStaus PaymentStaus { get; set; }

        // Method 
        public decimal TotalSalary()
        {
            return (Amount + Bonuses - Deductions);
        }

        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
