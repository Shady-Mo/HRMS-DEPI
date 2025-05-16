using System.ComponentModel.DataAnnotations;

namespace DEPI_Project.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string UserType { get; set; }
    }
}