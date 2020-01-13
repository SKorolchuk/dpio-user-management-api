using System.Collections.Generic;
using Deeproxio.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Deeproxio.Persistence.Identity.Models
{
    public class PlatformIdentityUser : IdentityUser, IEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public int Age { get; set; }

        public ICollection<UserSetting> Settings { get; set; }

        public bool Equals(PlatformIdentityUser other)
        {
            return other?.Id == Id;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PlatformIdentityUser)obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
