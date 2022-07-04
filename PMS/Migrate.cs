using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using PMS.Contexts;

namespace PMS.Migrate
{
    internal class Migrate
    {
        internal static int RunMigration()
        {
            UserContext context = new UserContext();
            try
            {
                RelationalDatabaseCreator databaseCreator =
                    (RelationalDatabaseCreator) context.Database.GetService<IDatabaseCreator>();
                databaseCreator.CreateTables();
            }
            catch
            {
                return -1;
            }

            return 0;
        }
    }
}