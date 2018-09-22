using System.ComponentModel.DataAnnotations;

namespace Deeproxio.AccountApi.Models
{
    public class UseRecoveryCodeViewModel
    {
        [Required]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }
    }
}
