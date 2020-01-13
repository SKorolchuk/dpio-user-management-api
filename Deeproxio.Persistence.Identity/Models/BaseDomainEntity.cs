using System;
using Deeproxio.Persistence.Identity.Models;

namespace Deeproxio.Domain.Models
{
    public abstract class BaseDomainEntity : IEntity, IEquatable<BaseDomainEntity>
    {
        public int Id { get; protected set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }

        public bool Equals(BaseDomainEntity other)
        {
            return other?.Id == Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BaseDomainEntity)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
