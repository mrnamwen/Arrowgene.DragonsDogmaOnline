using System;
using System.Collections.Generic;
using System.Data.Common;
using Arrowgene.Ddon.Database.Model;

namespace Arrowgene.Ddon.Database.Sql.Core;

public partial class DdonSqlDb : SqlDb
{
    protected static readonly string[] GpPurchaseHistoryFields = new[]
    {
        "character_id", "shop_type", "lineup_id", "item_id", "quantity", "gp_cost", "purchase_time"
    };

    private readonly string SqlInsertGpPurchaseHistory =
        $"INSERT INTO \"ddon_gp_purchase_history\" ({BuildQueryField(GpPurchaseHistoryFields)}) VALUES ({BuildQueryInsert(GpPurchaseHistoryFields)});";

    private readonly string SqlSelectGpPurchaseHistory = $@"
        SELECT ""id"", {BuildQueryField(GpPurchaseHistoryFields)} FROM ""ddon_gp_purchase_history""
        WHERE ""character_id"" = @character_id
        ORDER BY ""purchase_time"" DESC;";

    private readonly string SqlSelectGpPurchaseHistoryByShopType = $@"
        SELECT ""id"", {BuildQueryField(GpPurchaseHistoryFields)} FROM ""ddon_gp_purchase_history""
        WHERE ""character_id"" = @character_id AND ""shop_type"" = @shop_type
        ORDER BY ""purchase_time"" DESC;";

    public override long InsertGpPurchaseHistory(GpPurchaseHistory purchase, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteNonQuery(conn, SqlInsertGpPurchaseHistory, command =>
            {
                AddParameter(command, "character_id", purchase.CharacterId);
                AddParameter(command, "shop_type", purchase.ShopType);
                AddParameter(command, "lineup_id", purchase.LineupId);
                AddParameter(command, "item_id", purchase.ItemId);
                AddParameter(command, "quantity", purchase.Quantity);
                AddParameter(command, "gp_cost", purchase.GpCost);
                AddParameter(command, "purchase_time", purchase.PurchaseTime);
            }, out long autoIncrement);

            return autoIncrement;
        });
    }

    public override List<GpPurchaseHistory> SelectGpPurchaseHistory(uint characterId, DbConnection? connectionIn = null)
    {
        List<GpPurchaseHistory> results = new();

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectGpPurchaseHistory,
                command => { AddParameter(command, "character_id", characterId); },
                reader =>
                {
                    while (reader.Read())
                    {
                        results.Add(ReadGpPurchaseHistory(reader));
                    }
                });
        });

        return results;
    }

    public override List<GpPurchaseHistory> SelectGpPurchaseHistoryByShopType(uint characterId, uint shopType, DbConnection? connectionIn = null)
    {
        List<GpPurchaseHistory> results = new();

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectGpPurchaseHistoryByShopType,
                command =>
                {
                    AddParameter(command, "character_id", characterId);
                    AddParameter(command, "shop_type", shopType);
                },
                reader =>
                {
                    while (reader.Read())
                    {
                        results.Add(ReadGpPurchaseHistory(reader));
                    }
                });
        });

        return results;
    }

    private GpPurchaseHistory ReadGpPurchaseHistory(DbDataReader reader)
    {
        return new GpPurchaseHistory
        {
            Id = GetUInt64(reader, "id"),
            CharacterId = GetUInt32(reader, "character_id"),
            ShopType = GetUInt32(reader, "shop_type"),
            LineupId = GetUInt32(reader, "lineup_id"),
            ItemId = GetUInt32(reader, "item_id"),
            Quantity = GetUInt32(reader, "quantity"),
            GpCost = GetUInt32(reader, "gp_cost"),
            PurchaseTime = GetDateTime(reader, "purchase_time")
        };
    }
}
