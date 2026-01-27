using System.Data.Common;

namespace Arrowgene.Ddon.Database.Sql.Core.Migration
{
    public class InfinityDeliveryMigration(DatabaseSetting databaseSetting) : IMigrationStrategy
    {
        public uint From => 62;
        public uint To => 63;

        public bool Migrate(IDatabase db, DbConnection conn)
        {
            string adaptedSchema = DdonDatabaseBuilder.GetAdaptedSchema(databaseSetting, "Script/migration_infinity_delivery.sql");
            db.Execute(conn, adaptedSchema, true);
            return true;
        }
    }
}
