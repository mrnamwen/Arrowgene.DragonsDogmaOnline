using System.Data.Common;

namespace Arrowgene.Ddon.Database.Sql.Core;

public partial class DdonSqlDb : SqlDb
{
    private static readonly string SqlSelectGpShopPurchaseCount =
        @"SELECT ""purchase_count"" FROM ""ddon_gp_shop_purchase_count"" WHERE ""character_id"" = @character_id AND ""lineup_id"" = @lineup_id;";

    private static readonly string SqlUpsertGpShopPurchaseCount =
        @"INSERT INTO ""ddon_gp_shop_purchase_count"" (""character_id"", ""lineup_id"", ""purchase_count"")
          VALUES (@character_id, @lineup_id, @purchase_count)
          ON CONFLICT (""character_id"", ""lineup_id"")
          DO UPDATE SET ""purchase_count"" = @purchase_count;";

    public override uint SelectGpShopPurchaseCount(uint characterId, uint lineupId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            uint count = 0;
            ExecuteReader(connection, SqlSelectGpShopPurchaseCount, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "lineup_id", lineupId);
            }, reader =>
            {
                if (reader.Read())
                {
                    count = GetUInt32(reader, "purchase_count");
                }
            });
            return count;
        });
    }

    public override bool UpsertGpShopPurchaseCount(uint characterId, uint lineupId, uint purchaseCount, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            return ExecuteNonQuery(connection, SqlUpsertGpShopPurchaseCount, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "lineup_id", lineupId);
                AddParameter(command, "purchase_count", purchaseCount);
            }) >= 1;
        });
    }
}
