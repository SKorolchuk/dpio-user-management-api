namespace Deeproxio.Persistence.Identity.Migration.Managers
{
    public interface IMigrationManager
    {
        void Migrate(string[] args, bool withDataSeed = false);
    }
}
