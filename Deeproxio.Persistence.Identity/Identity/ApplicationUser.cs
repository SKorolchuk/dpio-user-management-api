using Microsoft.AspNetCore.Identity;

namespace Deeproxio.Persistence.Identity.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public bool Equals(ApplicationUser other)
        {
            return other?.Id == Id;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public int Age { get; set; }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ApplicationUser)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
