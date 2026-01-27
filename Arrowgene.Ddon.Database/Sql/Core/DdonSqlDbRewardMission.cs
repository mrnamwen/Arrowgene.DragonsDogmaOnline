using System;
using System.Collections.Generic;
using System.Data.Common;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.Database.Sql.Core;

public partial class DdonSqlDb : SqlDb
{
    // Reward Mission Progress SQL
    private static readonly string SqlSelectRewardMissionProgress =
        @"SELECT ""mission_id"", ""current_count"", ""is_complete"", ""is_received""
          FROM ""ddon_reward_mission_progress""
          WHERE ""character_id"" = @character_id;";

    private static readonly string SqlSelectRewardMissionProgressByMissionId =
        @"SELECT ""mission_id"", ""current_count"", ""is_complete"", ""is_received""
          FROM ""ddon_reward_mission_progress""
          WHERE ""character_id"" = @character_id AND ""mission_id"" = @mission_id;";

    private static readonly string SqlUpsertRewardMissionProgress =
        @"INSERT INTO ""ddon_reward_mission_progress"" (""character_id"", ""mission_id"", ""current_count"", ""is_complete"", ""is_received"")
          VALUES (@character_id, @mission_id, @current_count, @is_complete, @is_received)
          ON CONFLICT (""character_id"", ""mission_id"")
          DO UPDATE SET ""current_count"" = @current_count, ""is_complete"" = @is_complete, ""is_received"" = @is_received;";

    private static readonly string SqlDeleteRewardMissionProgress =
        @"DELETE FROM ""ddon_reward_mission_progress"" WHERE ""character_id"" = @character_id;";

    // Reward Mission State SQL
    private static readonly string SqlSelectRewardMissionState =
        @"SELECT ""last_reset_time"", ""missions_completed""
          FROM ""ddon_reward_mission_state""
          WHERE ""character_id"" = @character_id;";

    private static readonly string SqlUpsertRewardMissionState =
        @"INSERT INTO ""ddon_reward_mission_state"" (""character_id"", ""last_reset_time"", ""missions_completed"")
          VALUES (@character_id, @last_reset_time, @missions_completed)
          ON CONFLICT (""character_id"")
          DO UPDATE SET ""last_reset_time"" = @last_reset_time, ""missions_completed"" = @missions_completed;";

    // Reward Mission Milestone SQL
    private static readonly string SqlSelectRewardMissionMilestones =
        @"SELECT ""milestone_id"", ""is_received""
          FROM ""ddon_reward_mission_milestone""
          WHERE ""character_id"" = @character_id;";

    private static readonly string SqlUpsertRewardMissionMilestone =
        @"INSERT INTO ""ddon_reward_mission_milestone"" (""character_id"", ""milestone_id"", ""is_received"")
          VALUES (@character_id, @milestone_id, @is_received)
          ON CONFLICT (""character_id"", ""milestone_id"")
          DO UPDATE SET ""is_received"" = @is_received;";

    private static readonly string SqlDeleteRewardMissionMilestones =
        @"DELETE FROM ""ddon_reward_mission_milestone"" WHERE ""character_id"" = @character_id;";

    // Reward Mission Progress Methods
    public override List<RewardMissionProgress> SelectRewardMissionProgress(uint characterId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            List<RewardMissionProgress> result = new();
            ExecuteReader(connection, SqlSelectRewardMissionProgress, command =>
            {
                AddParameter(command, "character_id", characterId);
            }, reader =>
            {
                while (reader.Read())
                {
                    result.Add(ReadRewardMissionProgress(reader));
                }
            });
            return result;
        });
    }

    public override RewardMissionProgress? SelectRewardMissionProgressByMissionId(uint characterId, uint missionId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            RewardMissionProgress? result = null;
            ExecuteReader(connection, SqlSelectRewardMissionProgressByMissionId, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "mission_id", missionId);
            }, reader =>
            {
                if (reader.Read())
                {
                    result = ReadRewardMissionProgress(reader);
                }
            });
            return result;
        });
    }

    public override bool UpsertRewardMissionProgress(uint characterId, RewardMissionProgress progress, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            return ExecuteNonQuery(connection, SqlUpsertRewardMissionProgress, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "mission_id", progress.MissionId);
                AddParameter(command, "current_count", progress.CurrentCount);
                AddParameter(command, "is_complete", progress.IsComplete);
                AddParameter(command, "is_received", progress.IsReceived);
            }) >= 1;
        });
    }

    public override bool DeleteRewardMissionProgress(uint characterId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            return ExecuteNonQuery(connection, SqlDeleteRewardMissionProgress, command =>
            {
                AddParameter(command, "character_id", characterId);
            }) >= 0;
        });
    }

    private RewardMissionProgress ReadRewardMissionProgress(DbDataReader reader)
    {
        return new RewardMissionProgress
        {
            MissionId = GetUInt32(reader, "mission_id"),
            CurrentCount = GetUInt32(reader, "current_count"),
            IsComplete = GetBoolean(reader, "is_complete"),
            IsReceived = GetBoolean(reader, "is_received")
        };
    }

    // Reward Mission State Methods
    public override RewardMissionState SelectRewardMissionState(uint characterId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            RewardMissionState result = new();
            ExecuteReader(connection, SqlSelectRewardMissionState, command =>
            {
                AddParameter(command, "character_id", characterId);
            }, reader =>
            {
                if (reader.Read())
                {
                    result = ReadRewardMissionState(reader);
                }
            });
            return result;
        });
    }

    public override bool UpsertRewardMissionState(uint characterId, RewardMissionState state, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            return ExecuteNonQuery(connection, SqlUpsertRewardMissionState, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "last_reset_time", state.LastResetTime);
                AddParameter(command, "missions_completed", state.MissionsCompleted);
            }) >= 1;
        });
    }

    private RewardMissionState ReadRewardMissionState(DbDataReader reader)
    {
        return new RewardMissionState
        {
            LastResetTime = GetDateTime(reader, "last_reset_time"),
            MissionsCompleted = GetUInt32(reader, "missions_completed")
        };
    }

    // Reward Mission Milestone Methods
    public override List<RewardMissionMilestone> SelectRewardMissionMilestones(uint characterId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            List<RewardMissionMilestone> result = new();
            ExecuteReader(connection, SqlSelectRewardMissionMilestones, command =>
            {
                AddParameter(command, "character_id", characterId);
            }, reader =>
            {
                while (reader.Read())
                {
                    result.Add(ReadRewardMissionMilestone(reader));
                }
            });
            return result;
        });
    }

    public override bool UpsertRewardMissionMilestone(uint characterId, RewardMissionMilestone milestone, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            return ExecuteNonQuery(connection, SqlUpsertRewardMissionMilestone, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "milestone_id", milestone.MilestoneId);
                AddParameter(command, "is_received", milestone.IsReceived);
            }) >= 1;
        });
    }

    public override bool DeleteRewardMissionMilestones(uint characterId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, connection =>
        {
            return ExecuteNonQuery(connection, SqlDeleteRewardMissionMilestones, command =>
            {
                AddParameter(command, "character_id", characterId);
            }) >= 0;
        });
    }

    private RewardMissionMilestone ReadRewardMissionMilestone(DbDataReader reader)
    {
        return new RewardMissionMilestone
        {
            MilestoneId = GetUInt32(reader, "milestone_id"),
            IsReceived = GetBoolean(reader, "is_received")
        };
    }
}
