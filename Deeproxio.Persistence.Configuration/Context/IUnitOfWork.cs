using Microsoft.EntityFrameworkCore;

namespace Deeproxio.Persistence.Configuration.Context
{
	public interface IUnitOfWork
	{
		DbContext Context { get; }
	}
}