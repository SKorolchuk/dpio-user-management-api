using Deeproxio.AccountApi.Models.Validations;

namespace Deeproxio.AccountApi.Models
{
    [Validator(typeof(CredentialsViewModelValidator))]
    public class CredentialsViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
