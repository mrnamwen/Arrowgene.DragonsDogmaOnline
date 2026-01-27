using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Arrowgene.Ddon.Database.Sql.Core;

public partial class DdonSqlDb : SqlDb
{
    // Infinity Delivery Progress SQL
    private static readonly string SqlSelectInfinityDeliveryProgress =
        @"SELECT ""category_id"", ""total_points"", ""items_delivered"", ""period_start""
          FROM ""ddon_infinity_delivery_progress""
          WHERE ""character_id"" = @character_id;";

    private static readonly string SqlSelectInfinityDeliveryProgressByCategory =
        @"SELECT ""category_id"", ""total_points"", ""items_delivered"", ""period_start""
          FROM ""ddon_infinity_delivery_progress""
          WHERE ""character_id"" = @character_id AND ""category_id"" = @category_id;";

    private static readonly string SqlUpsertInfinityDeliveryProgress =
        @"INSERT INTO ""ddon_infinity_delivery_progress"" (""character_id"", ""category_id"", ""total_points"", ""items_delivered"", ""period_start"")
          VALUES (@character_id, @category_id, @total_points, @items_delivered, @period_start)
          ON CONFLICT (""character_id"", ""category_id"")
          DO UPDATE SET ""total_points"" = @total_points, ""items_delivered"" = @items_delivered, ""period_start"" = @period_start;";

    private static readonly string SqlDeleteInfinityDeliveryProgress =
        @"DELETE FROM ""ddon_infinity_delivery_progress"" WHERE ""character_id"" = @character_id;";

    private static readonly string SqlDeleteInfinityDeliveryProgressByCategory =
        @"DELETE FROM ""ddon_infinity_delivery_progress"" WHERE ""character_id"" = @character_id AND ""category_id"" = @category_id;";

    // Infinity Delivery Border Claim SQL
    private static readonly string SqlSelectInfinityDeliveryBorderClaims =
        @"SELECT ""border_id"", ""claimed_at"", ""period_start""
          FROM ""ddon_infinity_delivery_border_claim""
          WHERE ""character_id"" = @character_id;";

    private static readonly string SqlSelectInfinityDeliveryBorderClaimsByPeriod =
        @"SELECT ""border_id"", ""claimed_at"", ""period_start""
          FROM ""ddon_infinity_delivery_border_claim""
          WHERE ""character_id"" = @character_id AND ""period_start"" = @period_start;";

    private static readonly string SqlInsertInfinityDeliveryBorderClaim =
        @"INSERT INTO ""ddon_infinity_delivery_border_claim"" (""character_id"", ""border_id"", ""claimed_at"", ""period_start"")
          VALUES (@character_id, @border_id, @claimed_at, @period_start);";

    private static readonly string SqlDeleteInfinityDeliveryBorderClaims =
        @"DELETE FROM ""ddon_infinity_delivery_border_claim"" WHERE ""character_id"" = @character_id;";

    private static readonly string SqlDeleteInfinityDeliveryBorderClaimsByPeriod =
        @"DELETE FROM ""ddon_infinity_delivery_border_claim"" WHERE ""character_id"" = @character_id AND ""period_start"" = @period_start;";

    // Infinity Delivery Progress Methods
    public override List<(uint CategoryId, uint TotalPoints, uint ItemsDelivered, DateTime PeriodStart)> SelectInfinityDeliveryProgress(uint characterId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            List<(uint CategoryId, uint TotalPoints, uint ItemsDelivered, DateTime PeriodStart)> result = new();
            ExecuteReader(connection, SqlSelectInfinityDeliveryProgress, command =>
            {
                AddParameter(command, "character_id", characterId);
            }, reader =>
            {
                while (reader.Read())
                {
                    result.Add((
                        GetUInt32(reader, "category_id"),
                        GetUInt32(reader, "total_points"),
                        GetUInt32(reader, "items_delivered"),
                        GetDateTime(reader, "period_start")
                    ));
                }
            });
            return result;
        });
    }

    public override (uint CategoryId, uint TotalPoints, uint ItemsDelivered, DateTime PeriodStart)? SelectInfinityDeliveryProgressByCategory(uint characterId, uint categoryId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            (uint CategoryId, uint TotalPoints, uint ItemsDelivered, DateTime PeriodStart)? result = null;
            ExecuteReader(connection, SqlSelectInfinityDeliveryProgressByCategory, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "category_id", categoryId);
            }, reader =>
            {
                if (reader.Read())
                {
                    result = (
                        GetUInt32(reader, "category_id"),
                        GetUInt32(reader, "total_points"),
                        GetUInt32(reader, "items_delivered"),
                        GetDateTime(reader, "period_start")
                    );
                }
            });
            return result;
        });
    }

    public override bool UpsertInfinityDeliveryProgress(uint characterId, uint categoryId, uint totalPoints, uint itemsDelivered, DateTime periodStart, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            return ExecuteNonQuery(connection, SqlUpsertInfinityDeliveryProgress, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "category_id", categoryId);
                AddParameter(command, "total_points", totalPoints);
                AddParameter(command, "items_delivered", itemsDelivered);
                AddParameter(command, "period_start", periodStart);
            }) >= 1;
        });
    }

    public override bool DeleteInfinityDeliveryProgress(uint characterId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            return ExecuteNonQuery(connection, SqlDeleteInfinityDeliveryProgress, command =>
            {
                AddParameter(command, "character_id", characterId);
            }) >= 0;
        });
    }

    public override bool DeleteInfinityDeliveryProgressByCategory(uint characterId, uint categoryId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            return ExecuteNonQuery(connection, SqlDeleteInfinityDeliveryProgressByCategory, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "category_id", categoryId);
            }) >= 0;
        });
    }

    // Infinity Delivery Border Claim Methods
    public override List<(uint BorderId, DateTime ClaimedAt, DateTime PeriodStart)> SelectInfinityDeliveryBorderClaims(uint characterId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            List<(uint BorderId, DateTime ClaimedAt, DateTime PeriodStart)> result = new();
            ExecuteReader(connection, SqlSelectInfinityDeliveryBorderClaims, command =>
            {
                AddParameter(command, "character_id", characterId);
            }, reader =>
            {
                while (reader.Read())
                {
                    result.Add((
                        GetUInt32(reader, "border_id"),
                        GetDateTime(reader, "claimed_at"),
                        GetDateTime(reader, "period_start")
                    ));
                }
            });
            return result;
        });
    }

    public override List<(uint BorderId, DateTime ClaimedAt, DateTime PeriodStart)> SelectInfinityDeliveryBorderClaimsByPeriod(uint characterId, DateTime periodStart, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            List<(uint BorderId, DateTime ClaimedAt, DateTime PeriodStart)> result = new();
            ExecuteReader(connection, SqlSelectInfinityDeliveryBorderClaimsByPeriod, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "period_start", periodStart);
            }, reader =>
            {
                while (reader.Read())
                {
                    result.Add((
                        GetUInt32(reader, "border_id"),
                        GetDateTime(reader, "claimed_at"),
                        GetDateTime(reader, "period_start")
                    ));
                }
            });
            return result;
        });
    }

    public override bool InsertInfinityDeliveryBorderClaim(uint characterId, uint borderId, DateTime claimedAt, DateTime periodStart, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            return ExecuteNonQuery(connection, SqlInsertInfinityDeliveryBorderClaim, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "border_id", borderId);
                AddParameter(command, "claimed_at", claimedAt);
                AddParameter(command, "period_start", periodStart);
            }) >= 1;
        });
    }

    public override bool DeleteInfinityDeliveryBorderClaims(uint characterId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            return ExecuteNonQuery(connection, SqlDeleteInfinityDeliveryBorderClaims, command =>
            {
                AddParameter(command, "character_id", characterId);
            }) >= 0;
        });
    }

    public override bool DeleteInfinityDeliveryBorderClaimsByPeriod(uint characterId, DateTime periodStart, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            return ExecuteNonQuery(connection, SqlDeleteInfinityDeliveryBorderClaimsByPeriod, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "period_start", periodStart);
            }) >= 0;
        });
    }
}
