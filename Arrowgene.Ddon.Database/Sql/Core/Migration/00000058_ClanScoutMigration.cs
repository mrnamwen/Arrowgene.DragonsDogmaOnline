using System.Data.Common;

namespace Arrowgene.Ddon.Database.Sql.Core.Migration
{
    public class ClanScoutMigration(DatabaseSetting databaseSetting) : IMigrationStrategy
    {
        public uint From => 57;
        public uint To => 58;

        public bool Migrate(IDatabase db, DbConnection conn)
        {
            string adaptedSchema = DdonDatabaseBuilder.GetAdaptedSchema(databaseSetting, "Script/migration_clan_scout.sql");
            db.Execute(conn, adaptedSchema, true);
            return true;
        }
    }
}
