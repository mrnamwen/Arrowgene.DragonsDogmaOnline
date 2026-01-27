using System.Collections.Generic;
using System.Data.Common;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.Database.Sql.Core;

public partial class DdonSqlDb : SqlDb
{
    protected static readonly string[] PersonalMailFields = new[]
    {
        /* message_id */ "recipient_character_id", "sender_character_id", "message_state", "message_title", "message_body", "send_date"
    };

    private readonly string SqlDeletePersonalMailMessage = "DELETE FROM \"ddon_personal_mail\" WHERE \"message_id\"=@message_id;";

    private readonly string SqlInsertPersonalMailMessage =
        $"INSERT INTO \"ddon_personal_mail\" ({BuildQueryField(PersonalMailFields)}) VALUES ({BuildQueryInsert(PersonalMailFields)});";

    private readonly string SqlSelectPersonalMailMessage =
        $"SELECT \"message_id\", {BuildQueryField(PersonalMailFields)} FROM \"ddon_personal_mail\" WHERE \"message_id\" = @message_id;";

    private readonly string SqlSelectPersonalMailMessages =
        $"SELECT \"message_id\", {BuildQueryField(PersonalMailFields)} FROM \"ddon_personal_mail\" WHERE \"recipient_character_id\" = @character_id ORDER BY \"send_date\" DESC;";

    private readonly string SqlUpdatePersonalMailMessageState = "UPDATE \"ddon_personal_mail\" SET \"message_state\"=@message_state WHERE \"message_id\"=@message_id;";

    // SQL to get sender info for personal mail
    private readonly string SqlSelectPersonalMailMessagesWithSenderInfo = @"
        SELECT pm.""message_id"", pm.""recipient_character_id"", pm.""sender_character_id"", pm.""message_state"",
               pm.""message_title"", pm.""message_body"", pm.""send_date"",
               c.""first_name"" as sender_first_name,
               COALESCE(cl.""name"", '') as sender_clan_name
        FROM ""ddon_personal_mail"" pm
        LEFT JOIN ""ddon_character"" c ON pm.""sender_character_id"" = c.""character_id""
        LEFT JOIN ""ddon_clan_membership"" cm ON c.""character_id"" = cm.""character_id""
        LEFT JOIN ""ddon_clan_param"" cl ON cm.""clan_id"" = cl.""clan_id""
        WHERE pm.""recipient_character_id"" = @character_id
        ORDER BY pm.""send_date"" DESC;";

    private readonly string SqlSelectPersonalMailMessageWithSenderInfo = @"
        SELECT pm.""message_id"", pm.""recipient_character_id"", pm.""sender_character_id"", pm.""message_state"",
               pm.""message_title"", pm.""message_body"", pm.""send_date"",
               c.""first_name"" as sender_first_name,
               COALESCE(cl.""name"", '') as sender_clan_name
        FROM ""ddon_personal_mail"" pm
        LEFT JOIN ""ddon_character"" c ON pm.""sender_character_id"" = c.""character_id""
        LEFT JOIN ""ddon_clan_membership"" cm ON c.""character_id"" = cm.""character_id""
        LEFT JOIN ""ddon_clan_param"" cl ON cm.""clan_id"" = cl.""clan_id""
        WHERE pm.""message_id"" = @message_id;";

    public override long InsertPersonalMailMessage(PersonalMailMessage message)
    {
        using DbConnection connection = OpenNewConnection();
        return InsertPersonalMailMessage(connection, message);
    }

    public long InsertPersonalMailMessage(DbConnection connection, PersonalMailMessage message)
    {
        ExecuteNonQuery(connection, SqlInsertPersonalMailMessage, command =>
        {
            AddParameter(command, "recipient_character_id", message.RecipientCharacterId);
            AddParameter(command, "sender_character_id", message.SenderCharacterId);
            AddParameter(command, "message_state", (byte)message.MessageState);
            AddParameter(command, "message_title", message.Title);
            AddParameter(command, "message_body", message.Body);
            AddParameter(command, "send_date", message.SendDate);
        }, out long autoIncrement);

        return autoIncrement;
    }

    public override List<PersonalMailMessage> SelectPersonalMailMessages(uint characterId)
    {
        using DbConnection connection = OpenNewConnection();
        return SelectPersonalMailMessages(connection, characterId);
    }

    public List<PersonalMailMessage> SelectPersonalMailMessages(DbConnection conn, uint characterId)
    {
        List<PersonalMailMessage> results = new();

        ExecuteInTransaction(conn =>
        {
            ExecuteReader(conn, SqlSelectPersonalMailMessagesWithSenderInfo,
                command => { AddParameter(command, "@character_id", characterId); }, reader =>
                {
                    while (reader.Read())
                    {
                        PersonalMailMessage result = ReadPersonalMailMessageWithSenderInfo(reader);
                        results.Add(result);
                    }
                });
        });

        return results;
    }

    public override PersonalMailMessage SelectPersonalMailMessage(ulong messageId)
    {
        using DbConnection connection = OpenNewConnection();
        return SelectPersonalMailMessage(connection, messageId);
    }

    public PersonalMailMessage SelectPersonalMailMessage(DbConnection conn, ulong messageId)
    {
        PersonalMailMessage result = new();

        ExecuteInTransaction(conn =>
        {
            ExecuteReader(conn, SqlSelectPersonalMailMessageWithSenderInfo,
                command => { AddParameter(command, "@message_id", messageId); }, reader =>
                {
                    if (reader.Read()) result = ReadPersonalMailMessageWithSenderInfo(reader);
                });
        });

        return result;
    }

    public override bool UpdatePersonalMailMessageState(ulong messageId, MailState messageState)
    {
        using DbConnection connection = OpenNewConnection();
        return UpdatePersonalMailMessageState(connection, messageId, messageState);
    }

    public bool UpdatePersonalMailMessageState(DbConnection connection, ulong messageId, MailState messageState)
    {
        return ExecuteNonQuery(connection, SqlUpdatePersonalMailMessageState, command =>
        {
            AddParameter(command, "message_id", messageId);
            AddParameter(command, "message_state", (uint)messageState);
        }) == 1;
    }

    public override bool DeletePersonalMailMessage(ulong messageId)
    {
        using DbConnection connection = OpenNewConnection();
        return DeletePersonalMailMessage(connection, messageId);
    }

    public bool DeletePersonalMailMessage(DbConnection conn, ulong messageId)
    {
        return ExecuteNonQuery(conn, SqlDeletePersonalMailMessage, command => { AddParameter(command, "@message_id", messageId); }) == 1;
    }

    private PersonalMailMessage ReadPersonalMailMessage(DbDataReader reader)
    {
        PersonalMailMessage obj = new();
        obj.MessageId = GetUInt64(reader, "message_id");
        obj.RecipientCharacterId = GetUInt32(reader, "recipient_character_id");
        obj.SenderCharacterId = GetUInt32(reader, "sender_character_id");
        obj.MessageState = (MailState)GetUInt32(reader, "message_state");
        obj.Title = GetString(reader, "message_title");
        obj.Body = GetString(reader, "message_body");
        obj.SendDate = GetUInt64(reader, "send_date");
        return obj;
    }

    private PersonalMailMessage ReadPersonalMailMessageWithSenderInfo(DbDataReader reader)
    {
        PersonalMailMessage obj = ReadPersonalMailMessage(reader);
        obj.SenderName = GetString(reader, "sender_first_name");
        obj.SenderClanName = GetString(reader, "sender_clan_name");
        return obj;
    }
}
