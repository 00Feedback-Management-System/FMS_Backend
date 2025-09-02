using System.ComponentModel.DataAnnotations;

namespace Feedback_System.DTO
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string password { get; set; }

        //shri 

        [Required(ErrorMessage = "Role is required.")]
        public string role { get; set; }


    }
}
