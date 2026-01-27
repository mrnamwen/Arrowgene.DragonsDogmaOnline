using System.Data.Common;

namespace Arrowgene.Ddon.Database.Sql.Core.Migration
{
    public class DeathPenaltyPersistMigration(DatabaseSetting databaseSetting) : IMigrationStrategy
    {
        public uint From => 52;
        public uint To => 53;

        public bool Migrate(IDatabase db, DbConnection conn)
        {
            string adaptedSchema = DdonDatabaseBuilder.GetAdaptedSchema(databaseSetting, "Script/migration_death_penalty_persist.sql");
            db.Execute(conn, adaptedSchema, true);
            return true;
        }
    }
}
