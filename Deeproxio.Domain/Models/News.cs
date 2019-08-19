using System;

namespace Deeproxio.Domain.Models
{
	public class News : BaseEntity
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime CreateTS { get; set; }
		public DateTime UpdateTS { get; set; }
	}
}
