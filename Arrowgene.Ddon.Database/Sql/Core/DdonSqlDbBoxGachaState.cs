using System;
using System.Collections.Generic;
using System.Data.Common;
using Arrowgene.Ddon.Database.Model;

namespace Arrowgene.Ddon.Database.Sql.Core;

public partial class DdonSqlDb : SqlDb
{
    protected static readonly string[] BoxGachaStateFields = new[]
    {
        "character_id", "box_gacha_id", "lineup_id", "drawn_time"
    };

    private readonly string SqlInsertBoxGachaState =
        $"INSERT INTO \"ddon_box_gacha_state\" ({BuildQueryField(BoxGachaStateFields)}) VALUES ({BuildQueryInsert(BoxGachaStateFields)});";

    private readonly string SqlDeleteBoxGachaState = @"
        DELETE FROM ""ddon_box_gacha_state""
        WHERE ""character_id"" = @character_id AND ""box_gacha_id"" = @box_gacha_id;";

    private readonly string SqlSelectBoxGachaState = $@"
        SELECT {BuildQueryField(BoxGachaStateFields)} FROM ""ddon_box_gacha_state""
        WHERE ""character_id"" = @character_id AND ""box_gacha_id"" = @box_gacha_id
        ORDER BY ""lineup_id"" ASC;";

    private readonly string SqlSelectBoxGachaDrawnLineups = @"
        SELECT ""lineup_id"" FROM ""ddon_box_gacha_state""
        WHERE ""character_id"" = @character_id AND ""box_gacha_id"" = @box_gacha_id;";

    private readonly string SqlHasBoxGachaDrawnLineup = @"
        SELECT 1 FROM ""ddon_box_gacha_state""
        WHERE ""character_id"" = @character_id AND ""box_gacha_id"" = @box_gacha_id AND ""lineup_id"" = @lineup_id
        LIMIT 1;";

    public override bool InsertBoxGachaState(BoxGachaState state, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            return ExecuteNonQuery(conn, SqlInsertBoxGachaState, command =>
            {
                AddParameter(command, "character_id", state.CharacterId);
                AddParameter(command, "box_gacha_id", state.BoxGachaId);
                AddParameter(command, "lineup_id", state.LineupId);
                AddParameter(command, "drawn_time", state.DrawnTime);
            }) == 1;
        });
    }

    public override bool DeleteBoxGachaState(uint characterId, uint boxGachaId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            return ExecuteNonQuery(conn, SqlDeleteBoxGachaState, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "box_gacha_id", boxGachaId);
            }) >= 0;
        });
    }

    public override List<BoxGachaState> SelectBoxGachaState(uint characterId, uint boxGachaId, DbConnection? connectionIn = null)
    {
        List<BoxGachaState> results = new();

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectBoxGachaState,
                command =>
                {
                    AddParameter(command, "character_id", characterId);
                    AddParameter(command, "box_gacha_id", boxGachaId);
                },
                reader =>
                {
                    while (reader.Read())
                    {
                        results.Add(ReadBoxGachaState(reader));
                    }
                });
        });

        return results;
    }

    public override HashSet<uint> SelectBoxGachaDrawnLineups(uint characterId, uint boxGachaId, DbConnection? connectionIn = null)
    {
        HashSet<uint> results = new();

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectBoxGachaDrawnLineups,
                command =>
                {
                    AddParameter(command, "character_id", characterId);
                    AddParameter(command, "box_gacha_id", boxGachaId);
                },
                reader =>
                {
                    while (reader.Read())
                    {
                        results.Add(GetUInt32(reader, "lineup_id"));
                    }
                });
        });

        return results;
    }

    public override bool HasBoxGachaDrawnLineup(uint characterId, uint boxGachaId, uint lineupId, DbConnection? connectionIn = null)
    {
        bool exists = false;

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlHasBoxGachaDrawnLineup,
                command =>
                {
                    AddParameter(command, "character_id", characterId);
                    AddParameter(command, "box_gacha_id", boxGachaId);
                    AddParameter(command, "lineup_id", lineupId);
                },
                reader =>
                {
                    exists = reader.Read();
                });
        });

        return exists;
    }

    private BoxGachaState ReadBoxGachaState(DbDataReader reader)
    {
        return new BoxGachaState
        {
            CharacterId = GetUInt32(reader, "character_id"),
            BoxGachaId = GetUInt32(reader, "box_gacha_id"),
            LineupId = GetUInt32(reader, "lineup_id"),
            DrawnTime = GetDateTime(reader, "drawn_time")
        };
    }
}
