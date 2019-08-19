using System.ComponentModel.DataAnnotations;

namespace Deeproxio.UserManagement.API.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
