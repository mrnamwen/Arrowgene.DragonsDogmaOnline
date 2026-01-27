using System.Data.Common;

namespace Arrowgene.Ddon.Database.Sql.Core.Migration
{
    public class ReviveTimerMigration(DatabaseSetting databaseSetting) : IMigrationStrategy
    {
        public uint From => 53;
        public uint To => 54;

        public bool Migrate(IDatabase db, DbConnection conn)
        {
            string adaptedSchema = DdonDatabaseBuilder.GetAdaptedSchema(databaseSetting, "Script/migration_revive_timer.sql");
            db.Execute(conn, adaptedSchema, true);
            return true;
        }
    }
}
