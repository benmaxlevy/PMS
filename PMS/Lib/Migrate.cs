using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using PMS.Contexts;

namespace PMS.Lib
{
    internal class Migration
    {
        internal static int RunMigration()
        {
            try
            {
                var context = new UserContext();
                var databaseCreator =
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