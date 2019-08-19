using System;

namespace Deeproxio.Domain.Models
{
	public class User : BaseEntity, IEquatable<User>
	{
		public bool Equals(User other)
		{
			return other?.Id == Id;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((User)obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
