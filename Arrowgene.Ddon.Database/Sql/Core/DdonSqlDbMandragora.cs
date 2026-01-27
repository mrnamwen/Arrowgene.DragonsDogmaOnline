using System.Collections.Generic;
using System.Data.Common;
using Arrowgene.Ddon.Database.Model;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.Database.Sql.Core;

public partial class DdonSqlDb : SqlDb
{
    #region Mandragora Ownership

    protected static readonly string[] MandragoraFields = new[]
    {
        "character_id", "mandragora_id", "name", "species_index",
        "species_category", "furniture_item_id", "growth_level"
    };

    private readonly string SqlInsertMandragora =
        $"INSERT INTO \"ddon_mandragora\" ({BuildQueryField(MandragoraFields)}) VALUES ({BuildQueryInsert(MandragoraFields)});";

    private readonly string SqlUpdateMandragora = @"
        UPDATE ""ddon_mandragora"" SET
            ""name""=@name, ""species_index""=@species_index, ""species_category""=@species_category,
            ""furniture_item_id""=@furniture_item_id, ""growth_level""=@growth_level
        WHERE ""character_id""=@character_id AND ""mandragora_id""=@mandragora_id;";

    private readonly string SqlDeleteMandragora = @"
        DELETE FROM ""ddon_mandragora""
        WHERE ""character_id""=@character_id AND ""mandragora_id""=@mandragora_id;";

    private readonly string SqlSelectMandragoras = $@"
        SELECT {BuildQueryField(MandragoraFields)} FROM ""ddon_mandragora""
        WHERE ""character_id""=@character_id ORDER BY ""mandragora_id"" ASC;";

    private readonly string SqlSelectMandragora = $@"
        SELECT {BuildQueryField(MandragoraFields)} FROM ""ddon_mandragora""
        WHERE ""character_id""=@character_id AND ""mandragora_id""=@mandragora_id;";

    public override bool InsertMandragora(Mandragora mandragora, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            return ExecuteNonQuery(conn, SqlInsertMandragora, command => AddMandragoraParams(command, mandragora)) == 1;
        });
    }

    public override bool UpdateMandragora(Mandragora mandragora, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            return ExecuteNonQuery(conn, SqlUpdateMandragora, command => AddMandragoraParams(command, mandragora)) == 1;
        });
    }

    public override bool DeleteMandragora(uint characterId, uint mandragoraId, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            return ExecuteNonQuery(conn, SqlDeleteMandragora, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "mandragora_id", mandragoraId);
            }) == 1;
        });
    }

    public override List<Mandragora> SelectMandragoras(uint characterId, DbConnection? connectionIn = null)
    {
        List<Mandragora> results = new();

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectMandragoras,
                command => { AddParameter(command, "character_id", characterId); },
                reader =>
                {
                    while (reader.Read())
                    {
                        results.Add(ReadMandragora(reader));
                    }
                });
        });

        return results;
    }

    public override Mandragora? SelectMandragora(uint characterId, uint mandragoraId, DbConnection? connectionIn = null)
    {
        Mandragora? result = null;

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectMandragora,
                command =>
                {
                    AddParameter(command, "character_id", characterId);
                    AddParameter(command, "mandragora_id", mandragoraId);
                },
                reader =>
                {
                    if (reader.Read())
                    {
                        result = ReadMandragora(reader);
                    }
                });
        });

        return result;
    }

    private void AddMandragoraParams(DbCommand command, Mandragora mandragora)
    {
        AddParameter(command, "character_id", mandragora.CharacterId);
        AddParameter(command, "mandragora_id", mandragora.MandragoraId);
        AddParameter(command, "name", mandragora.Name);
        AddParameter(command, "species_index", mandragora.SpeciesIndex);
        AddParameter(command, "species_category", (byte)mandragora.SpeciesCategory);
        AddParameter(command, "furniture_item_id", mandragora.FurnitureItemId);
        AddParameter(command, "growth_level", mandragora.GrowthLevel);
    }

    private Mandragora ReadMandragora(DbDataReader reader)
    {
        return new Mandragora
        {
            CharacterId = GetUInt32(reader, "character_id"),
            MandragoraId = GetUInt32(reader, "mandragora_id"),
            Name = GetString(reader, "name"),
            SpeciesIndex = GetUInt32(reader, "species_index"),
            SpeciesCategory = (MandragoraSpeciesCategory)GetByte(reader, "species_category"),
            FurnitureItemId = GetUInt32(reader, "furniture_item_id"),
            GrowthLevel = GetUInt32(reader, "growth_level")
        };
    }

    #endregion

    #region Mandragora Species Discovery

    protected static readonly string[] MandragoraSpeciesDiscoveryFields = new[]
    {
        "character_id", "species_index", "species_category", "rarity", "is_new", "discovered_date"
    };

    private readonly string SqlInsertMandragoraSpeciesDiscovery =
        $"INSERT INTO \"ddon_mandragora_species_discovery\" ({BuildQueryField(MandragoraSpeciesDiscoveryFields)}) VALUES ({BuildQueryInsert(MandragoraSpeciesDiscoveryFields)});";

    private readonly string SqlUpdateMandragoraSpeciesDiscovery = @"
        UPDATE ""ddon_mandragora_species_discovery"" SET
            ""species_category""=@species_category, ""rarity""=@rarity, ""is_new""=@is_new, ""discovered_date""=@discovered_date
        WHERE ""character_id""=@character_id AND ""species_index""=@species_index;";

    private readonly string SqlSelectMandragoraSpeciesDiscoveries = $@"
        SELECT {BuildQueryField(MandragoraSpeciesDiscoveryFields)} FROM ""ddon_mandragora_species_discovery""
        WHERE ""character_id""=@character_id ORDER BY ""species_index"" ASC;";

    private readonly string SqlSelectMandragoraSpeciesDiscoveriesByCategory = $@"
        SELECT {BuildQueryField(MandragoraSpeciesDiscoveryFields)} FROM ""ddon_mandragora_species_discovery""
        WHERE ""character_id""=@character_id AND ""species_category""=@species_category
        ORDER BY ""species_index"" ASC;";

    public override bool InsertMandragoraSpeciesDiscovery(MandragoraSpeciesDiscovery discovery, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            return ExecuteNonQuery(conn, SqlInsertMandragoraSpeciesDiscovery, command => AddMandragoraSpeciesDiscoveryParams(command, discovery)) == 1;
        });
    }

    public override bool UpdateMandragoraSpeciesDiscovery(MandragoraSpeciesDiscovery discovery, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            return ExecuteNonQuery(conn, SqlUpdateMandragoraSpeciesDiscovery, command => AddMandragoraSpeciesDiscoveryParams(command, discovery)) == 1;
        });
    }

    public override List<MandragoraSpeciesDiscovery> SelectMandragoraSpeciesDiscoveries(uint characterId, DbConnection? connectionIn = null)
    {
        List<MandragoraSpeciesDiscovery> results = new();

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectMandragoraSpeciesDiscoveries,
                command => { AddParameter(command, "character_id", characterId); },
                reader =>
                {
                    while (reader.Read())
                    {
                        results.Add(ReadMandragoraSpeciesDiscovery(reader));
                    }
                });
        });

        return results;
    }

    public override List<MandragoraSpeciesDiscovery> SelectMandragoraSpeciesDiscoveriesByCategory(uint characterId, MandragoraSpeciesCategory category, DbConnection? connectionIn = null)
    {
        List<MandragoraSpeciesDiscovery> results = new();

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectMandragoraSpeciesDiscoveriesByCategory,
                command =>
                {
                    AddParameter(command, "character_id", characterId);
                    AddParameter(command, "species_category", (byte)category);
                },
                reader =>
                {
                    while (reader.Read())
                    {
                        results.Add(ReadMandragoraSpeciesDiscovery(reader));
                    }
                });
        });

        return results;
    }

    private void AddMandragoraSpeciesDiscoveryParams(DbCommand command, MandragoraSpeciesDiscovery discovery)
    {
        AddParameter(command, "character_id", discovery.CharacterId);
        AddParameter(command, "species_index", discovery.SpeciesIndex);
        AddParameter(command, "species_category", (byte)discovery.SpeciesCategory);
        AddParameter(command, "rarity", (byte)discovery.Rarity);
        AddParameter(command, "is_new", discovery.IsNew);
        AddParameter(command, "discovered_date", (uint)discovery.DiscoveredDate);
    }

    private MandragoraSpeciesDiscovery ReadMandragoraSpeciesDiscovery(DbDataReader reader)
    {
        return new MandragoraSpeciesDiscovery
        {
            CharacterId = GetUInt32(reader, "character_id"),
            SpeciesIndex = GetUInt32(reader, "species_index"),
            SpeciesCategory = (MandragoraSpeciesCategory)GetByte(reader, "species_category"),
            Rarity = (MandragoraRarity)GetByte(reader, "rarity"),
            IsNew = GetBoolean(reader, "is_new"),
            DiscoveredDate = GetInt64(reader, "discovered_date")
        };
    }

    #endregion

    #region Mandragora First Discovery (Server-Wide)

    private readonly string SqlInsertOrIgnoreFirstDiscovery = @"
        INSERT OR IGNORE INTO ""ddon_mandragora_first_discovery""
        (""species_index"", ""character_id"", ""character_name"", ""discovered_date"")
        VALUES (@species_index, @character_id, @character_name, @discovered_date);";

    private readonly string SqlSelectFirstDiscovery = @"
        SELECT ""character_name"" FROM ""ddon_mandragora_first_discovery""
        WHERE ""species_index""=@species_index;";

    public override bool InsertOrIgnoreMandragoraFirstDiscovery(uint speciesIndex, uint characterId, string characterName, long discoveredDate, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            return ExecuteNonQuery(conn, SqlInsertOrIgnoreFirstDiscovery, command =>
            {
                AddParameter(command, "species_index", speciesIndex);
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "character_name", characterName);
                AddParameter(command, "discovered_date", (uint)discoveredDate);
            }) == 1;
        });
    }

    public override string? SelectMandragoraFirstDiscoverer(uint speciesIndex, DbConnection? connectionIn = null)
    {
        string? result = null;

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectFirstDiscovery,
                command => { AddParameter(command, "species_index", speciesIndex); },
                reader =>
                {
                    if (reader.Read())
                    {
                        result = GetString(reader, "character_name");
                    }
                });
        });

        return result;
    }

    #endregion
}
