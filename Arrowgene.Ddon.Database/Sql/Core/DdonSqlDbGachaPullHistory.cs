using System;
using System.Collections.Generic;
using System.Data.Common;
using Arrowgene.Ddon.Database.Model;

namespace Arrowgene.Ddon.Database.Sql.Core;

public partial class DdonSqlDb : SqlDb
{
    protected static readonly string[] GachaPullHistoryFields = new[]
    {
        "character_id", "gacha_id", "pull_type", "currency_type", "currency_cost", "pull_count", "pull_time"
    };

    protected static readonly string[] GachaPullResultFields = new[]
    {
        "pull_id", "result_index", "item_id", "quantity", "rarity", "is_bonus"
    };

    private readonly string SqlInsertGachaPullHistory =
        $"INSERT INTO \"ddon_gacha_pull_history\" ({BuildQueryField(GachaPullHistoryFields)}) VALUES ({BuildQueryInsert(GachaPullHistoryFields)});";

    private readonly string SqlInsertGachaPullResult =
        $"INSERT INTO \"ddon_gacha_pull_result\" ({BuildQueryField(GachaPullResultFields)}) VALUES ({BuildQueryInsert(GachaPullResultFields)});";

    private readonly string SqlSelectGachaPullHistory = $@"
        SELECT ""id"", {BuildQueryField(GachaPullHistoryFields)} FROM ""ddon_gacha_pull_history""
        WHERE ""character_id"" = @character_id
        ORDER BY ""pull_time"" DESC;";

    private readonly string SqlSelectGachaPullHistoryByGachaId = $@"
        SELECT ""id"", {BuildQueryField(GachaPullHistoryFields)} FROM ""ddon_gacha_pull_history""
        WHERE ""character_id"" = @character_id AND ""gacha_id"" = @gacha_id
        ORDER BY ""pull_time"" DESC;";

    private readonly string SqlSelectGachaPullResults = $@"
        SELECT ""id"", {BuildQueryField(GachaPullResultFields)} FROM ""ddon_gacha_pull_result""
        WHERE ""pull_id"" = @pull_id
        ORDER BY ""result_index"" ASC;";

    public override long InsertGachaPullHistory(GachaPullHistory pull, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteNonQuery(conn, SqlInsertGachaPullHistory, command =>
            {
                AddParameter(command, "character_id", pull.CharacterId);
                AddParameter(command, "gacha_id", pull.GachaId);
                AddParameter(command, "pull_type", pull.PullType);
                AddParameter(command, "currency_type", pull.CurrencyType);
                AddParameter(command, "currency_cost", pull.CurrencyCost);
                AddParameter(command, "pull_count", pull.PullCount);
                AddParameter(command, "pull_time", pull.PullTime);
            }, out long autoIncrement);

            return autoIncrement;
        });
    }

    public override long InsertGachaPullResult(GachaPullResult result, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteNonQuery(conn, SqlInsertGachaPullResult, command =>
            {
                AddParameter(command, "pull_id", (long)result.PullId);
                AddParameter(command, "result_index", result.ResultIndex);
                AddParameter(command, "item_id", result.ItemId);
                AddParameter(command, "quantity", result.Quantity);
                AddParameter(command, "rarity", result.Rarity);
                AddParameter(command, "is_bonus", result.IsBonus);
            }, out long autoIncrement);

            return autoIncrement;
        });
    }

    public override List<GachaPullHistory> SelectGachaPullHistory(uint characterId, DbConnection? connectionIn = null)
    {
        List<GachaPullHistory> results = new();

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectGachaPullHistory,
                command => { AddParameter(command, "character_id", characterId); },
                reader =>
                {
                    while (reader.Read())
                    {
                        results.Add(ReadGachaPullHistory(reader));
                    }
                });
        });

        return results;
    }

    public override List<GachaPullHistory> SelectGachaPullHistoryByGachaId(uint characterId, uint gachaId, DbConnection? connectionIn = null)
    {
        List<GachaPullHistory> results = new();

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectGachaPullHistoryByGachaId,
                command =>
                {
                    AddParameter(command, "character_id", characterId);
                    AddParameter(command, "gacha_id", gachaId);
                },
                reader =>
                {
                    while (reader.Read())
                    {
                        results.Add(ReadGachaPullHistory(reader));
                    }
                });
        });

        return results;
    }

    public override List<GachaPullResult> SelectGachaPullResults(ulong pullId, DbConnection? connectionIn = null)
    {
        List<GachaPullResult> results = new();

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectGachaPullResults,
                command => { AddParameter(command, "pull_id", (long)pullId); },
                reader =>
                {
                    while (reader.Read())
                    {
                        results.Add(ReadGachaPullResult(reader));
                    }
                });
        });

        return results;
    }

    private GachaPullHistory ReadGachaPullHistory(DbDataReader reader)
    {
        return new GachaPullHistory
        {
            Id = GetUInt64(reader, "id"),
            CharacterId = GetUInt32(reader, "character_id"),
            GachaId = GetUInt32(reader, "gacha_id"),
            PullType = GetUInt32(reader, "pull_type"),
            CurrencyType = GetUInt32(reader, "currency_type"),
            CurrencyCost = GetUInt32(reader, "currency_cost"),
            PullCount = GetUInt32(reader, "pull_count"),
            PullTime = GetDateTime(reader, "pull_time")
        };
    }

    private GachaPullResult ReadGachaPullResult(DbDataReader reader)
    {
        return new GachaPullResult
        {
            Id = GetUInt64(reader, "id"),
            PullId = GetUInt64(reader, "pull_id"),
            ResultIndex = GetUInt32(reader, "result_index"),
            ItemId = GetUInt32(reader, "item_id"),
            Quantity = GetUInt32(reader, "quantity"),
            Rarity = GetUInt32(reader, "rarity"),
            IsBonus = GetBoolean(reader, "is_bonus")
        };
    }
}
