using System;
using System.Collections.Generic;
using System.Data.Common;
using Arrowgene.Ddon.Database.Model;

namespace Arrowgene.Ddon.Database.Sql.Core;

public partial class DdonSqlDb : SqlDb
{
    // Character Available Courses
    protected static readonly string[] CharacterAvailableCourseFields = new[]
    {
        "character_id", "course_id", "course_name", "duration_sec", "lineup_id", "back_icon_id", "frame_icon_id", "purchase_time"
    };

    private readonly string SqlInsertCharacterAvailableCourse =
        $"INSERT INTO \"ddon_character_course_available\" ({BuildQueryField(CharacterAvailableCourseFields)}) VALUES ({BuildQueryInsert(CharacterAvailableCourseFields)});";

    private readonly string SqlSelectCharacterAvailableCourses = $@"
        SELECT ""id"", {BuildQueryField(CharacterAvailableCourseFields)} FROM ""ddon_character_course_available""
        WHERE ""character_id"" = @character_id
        ORDER BY ""purchase_time"" ASC;";

    private readonly string SqlSelectCharacterAvailableCourseById = $@"
        SELECT ""id"", {BuildQueryField(CharacterAvailableCourseFields)} FROM ""ddon_character_course_available""
        WHERE ""id"" = @id;";

    private readonly string SqlDeleteCharacterAvailableCourse = @"
        DELETE FROM ""ddon_character_course_available"" WHERE ""id"" = @id;";

    // Character Active Courses
    protected static readonly string[] CharacterActiveCourseFields = new[]
    {
        "character_id", "course_id", "course_name", "start_time", "end_time"
    };

    private readonly string SqlInsertCharacterActiveCourse =
        $"INSERT INTO \"ddon_character_course_active\" ({BuildQueryField(CharacterActiveCourseFields)}) VALUES ({BuildQueryInsert(CharacterActiveCourseFields)});";

    private readonly string SqlSelectCharacterActiveCourses = $@"
        SELECT ""id"", {BuildQueryField(CharacterActiveCourseFields)} FROM ""ddon_character_course_active""
        WHERE ""character_id"" = @character_id
        ORDER BY ""end_time"" ASC;";

    private readonly string SqlSelectCharacterActiveCoursesByCourseId = $@"
        SELECT ""id"", {BuildQueryField(CharacterActiveCourseFields)} FROM ""ddon_character_course_active""
        WHERE ""character_id"" = @character_id AND ""course_id"" = @course_id
        ORDER BY ""end_time"" ASC;";

    private readonly string SqlUpdateCharacterActiveCourse = $@"
        UPDATE ""ddon_character_course_active"" SET
            ""start_time"" = @start_time,
            ""end_time"" = @end_time
        WHERE ""id"" = @id;";

    private readonly string SqlDeleteExpiredCharacterActiveCourses = @"
        DELETE FROM ""ddon_character_course_active"" WHERE ""end_time"" < @current_time;";

    private readonly string SqlDeleteCharacterActiveCourse = @"
        DELETE FROM ""ddon_character_course_active"" WHERE ""id"" = @id;";

    // Character Available Course Methods
    public override long InsertCharacterAvailableCourse(CharacterAvailableCourse course, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteNonQuery(conn, SqlInsertCharacterAvailableCourse, command =>
            {
                AddParameter(command, "character_id", course.CharacterId);
                AddParameter(command, "course_id", course.CourseId);
                AddParameter(command, "course_name", course.CourseName);
                AddParameter(command, "duration_sec", course.DurationSec);
                AddParameter(command, "lineup_id", course.LineupId);
                AddParameter(command, "back_icon_id", course.BackIconId);
                AddParameter(command, "frame_icon_id", course.FrameIconId);
                AddParameter(command, "purchase_time", course.PurchaseTime);
            }, out long autoIncrement);

            return autoIncrement;
        });
    }

    public override List<CharacterAvailableCourse> SelectCharacterAvailableCourses(uint characterId, DbConnection? connectionIn = null)
    {
        List<CharacterAvailableCourse> results = new();

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectCharacterAvailableCourses,
                command => { AddParameter(command, "character_id", characterId); },
                reader =>
                {
                    while (reader.Read())
                    {
                        results.Add(ReadCharacterAvailableCourse(reader));
                    }
                });
        });

        return results;
    }

    public override CharacterAvailableCourse? SelectCharacterAvailableCourseById(ulong id, DbConnection? connectionIn = null)
    {
        CharacterAvailableCourse? result = null;

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectCharacterAvailableCourseById,
                command => { AddParameter(command, "id", (long)id); },
                reader =>
                {
                    if (reader.Read())
                    {
                        result = ReadCharacterAvailableCourse(reader);
                    }
                });
        });

        return result;
    }

    public override bool DeleteCharacterAvailableCourse(ulong id, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            int rowsAffected = ExecuteNonQuery(conn, SqlDeleteCharacterAvailableCourse, command =>
            {
                AddParameter(command, "id", (long)id);
            });
            return rowsAffected > 0;
        });
    }

    private CharacterAvailableCourse ReadCharacterAvailableCourse(DbDataReader reader)
    {
        return new CharacterAvailableCourse
        {
            Id = GetUInt64(reader, "id"),
            CharacterId = GetUInt32(reader, "character_id"),
            CourseId = GetUInt32(reader, "course_id"),
            CourseName = GetString(reader, "course_name"),
            DurationSec = GetUInt32(reader, "duration_sec"),
            LineupId = GetUInt32(reader, "lineup_id"),
            BackIconId = GetUInt32(reader, "back_icon_id"),
            FrameIconId = GetUInt32(reader, "frame_icon_id"),
            PurchaseTime = GetDateTime(reader, "purchase_time")
        };
    }

    // Character Active Course Methods
    public override long InsertCharacterActiveCourse(CharacterActiveCourse course, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteNonQuery(conn, SqlInsertCharacterActiveCourse, command =>
            {
                AddParameter(command, "character_id", course.CharacterId);
                AddParameter(command, "course_id", course.CourseId);
                AddParameter(command, "course_name", course.CourseName);
                AddParameter(command, "start_time", course.StartTime);
                AddParameter(command, "end_time", course.EndTime);
            }, out long autoIncrement);

            return autoIncrement;
        });
    }

    public override List<CharacterActiveCourse> SelectCharacterActiveCourses(uint characterId, DbConnection? connectionIn = null)
    {
        List<CharacterActiveCourse> results = new();

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectCharacterActiveCourses,
                command => { AddParameter(command, "character_id", characterId); },
                reader =>
                {
                    while (reader.Read())
                    {
                        results.Add(ReadCharacterActiveCourse(reader));
                    }
                });
        });

        return results;
    }

    public override List<CharacterActiveCourse> SelectCharacterActiveCoursesByCourseId(uint characterId, uint courseId, DbConnection? connectionIn = null)
    {
        List<CharacterActiveCourse> results = new();

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectCharacterActiveCoursesByCourseId,
                command =>
                {
                    AddParameter(command, "character_id", characterId);
                    AddParameter(command, "course_id", courseId);
                },
                reader =>
                {
                    while (reader.Read())
                    {
                        results.Add(ReadCharacterActiveCourse(reader));
                    }
                });
        });

        return results;
    }

    public override bool UpdateCharacterActiveCourse(CharacterActiveCourse course, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            int rowsAffected = ExecuteNonQuery(conn, SqlUpdateCharacterActiveCourse, command =>
            {
                AddParameter(command, "id", (long)course.Id);
                AddParameter(command, "start_time", course.StartTime);
                AddParameter(command, "end_time", course.EndTime);
            });
            return rowsAffected > 0;
        });
    }

    public override int DeleteExpiredCharacterActiveCourses(long currentTime, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            return ExecuteNonQuery(conn, SqlDeleteExpiredCharacterActiveCourses, command =>
            {
                AddParameter(command, "current_time", currentTime);
            });
        });
    }

    public override bool DeleteCharacterActiveCourse(ulong id, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            int rowsAffected = ExecuteNonQuery(conn, SqlDeleteCharacterActiveCourse, command =>
            {
                AddParameter(command, "id", (long)id);
            });
            return rowsAffected > 0;
        });
    }

    private CharacterActiveCourse ReadCharacterActiveCourse(DbDataReader reader)
    {
        return new CharacterActiveCourse
        {
            Id = GetUInt64(reader, "id"),
            CharacterId = GetUInt32(reader, "character_id"),
            CourseId = GetUInt32(reader, "course_id"),
            CourseName = GetString(reader, "course_name"),
            StartTime = GetInt64(reader, "start_time"),
            EndTime = GetInt64(reader, "end_time")
        };
    }
}
