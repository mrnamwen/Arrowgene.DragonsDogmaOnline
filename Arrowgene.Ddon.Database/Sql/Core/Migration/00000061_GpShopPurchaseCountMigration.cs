using System.Data.Common;

namespace Arrowgene.Ddon.Database.Sql.Core.Migration
{
    public class GpShopPurchaseCountMigration(DatabaseSetting databaseSetting) : IMigrationStrategy
    {
        public uint From => 60;
        public uint To => 61;

        public bool Migrate(IDatabase db, DbConnection conn)
        {
            string adaptedSchema = DdonDatabaseBuilder.GetAdaptedSchema(databaseSetting, "Script/migration_gp_shop_purchase_count.sql");
            db.Execute(conn, adaptedSchema, true);
            return true;
        }
    }
}
