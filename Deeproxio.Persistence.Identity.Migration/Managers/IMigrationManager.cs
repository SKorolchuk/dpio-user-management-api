using Microsoft.Extensions.Configuration;

namespace Deeproxio.Persistence.Identity.Migrator.Managers
{
    public interface IMigrationManager
    {
        void Migrate(string[] args, bool withDataSeed = false);
    }
}
