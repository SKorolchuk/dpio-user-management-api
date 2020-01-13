using Deeproxio.Persistence.Identity.Models;

namespace Deeproxio.Domain.Models
{
    public class UserSetting : BaseDomainEntity, IEntity
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public PlatformIdentityUser SettingFor { get; set; }
    }
}
