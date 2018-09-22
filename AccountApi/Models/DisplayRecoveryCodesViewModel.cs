using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Deeproxio.AccountApi.Models
{
    public class DisplayRecoveryCodesViewModel
    {
        [Required]
        public IEnumerable<string> Codes { get; set; }

    }
}
