using System.ComponentModel.DataAnnotations;

namespace Deeproxio.AccountApi.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
