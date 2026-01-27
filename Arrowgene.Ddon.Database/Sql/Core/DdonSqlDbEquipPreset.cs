using System.Collections.Generic;
using System.Data.Common;
using Arrowgene.Ddon.Database.Model;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.Database.Sql.Core;

public partial class DdonSqlDb : SqlDb
{
    protected static readonly string[] EquipPresetFields = new[]
    {
        "character_id", "job", "preset_no", "preset_name",
        "p_primary_weapon", "p_secondary_weapon", "p_head", "p_body", "p_clothing",
        "p_arm", "p_leg", "p_leg_wear", "p_over_wear",
        "p_jewelry1", "p_jewelry2", "p_jewelry3", "p_jewelry4", "p_jewelry5", "p_lantern",
        "v_primary_weapon", "v_secondary_weapon", "v_head", "v_body", "v_clothing",
        "v_arm", "v_leg", "v_leg_wear", "v_over_wear"
    };

    private readonly string SqlInsertEquipPreset =
        $"INSERT INTO \"ddon_equip_preset\" ({BuildQueryField(EquipPresetFields)}) VALUES ({BuildQueryInsert(EquipPresetFields)});";

    private readonly string SqlUpdateEquipPreset = @"
        UPDATE ""ddon_equip_preset"" SET
            ""preset_name""=@preset_name,
            ""p_primary_weapon""=@p_primary_weapon, ""p_secondary_weapon""=@p_secondary_weapon,
            ""p_head""=@p_head, ""p_body""=@p_body, ""p_clothing""=@p_clothing,
            ""p_arm""=@p_arm, ""p_leg""=@p_leg, ""p_leg_wear""=@p_leg_wear, ""p_over_wear""=@p_over_wear,
            ""p_jewelry1""=@p_jewelry1, ""p_jewelry2""=@p_jewelry2, ""p_jewelry3""=@p_jewelry3,
            ""p_jewelry4""=@p_jewelry4, ""p_jewelry5""=@p_jewelry5, ""p_lantern""=@p_lantern,
            ""v_primary_weapon""=@v_primary_weapon, ""v_secondary_weapon""=@v_secondary_weapon,
            ""v_head""=@v_head, ""v_body""=@v_body, ""v_clothing""=@v_clothing,
            ""v_arm""=@v_arm, ""v_leg""=@v_leg, ""v_leg_wear""=@v_leg_wear, ""v_over_wear""=@v_over_wear
        WHERE ""character_id""=@character_id AND ""job""=@job AND ""preset_no""=@preset_no;";

    private readonly string SqlUpdateEquipPresetName = @"
        UPDATE ""ddon_equip_preset"" SET ""preset_name""=@preset_name
        WHERE ""character_id""=@character_id AND ""job""=@job AND ""preset_no""=@preset_no;";

    private readonly string SqlDeleteEquipPreset = @"
        DELETE FROM ""ddon_equip_preset""
        WHERE ""character_id""=@character_id AND ""job""=@job AND ""preset_no""=@preset_no;";

    private readonly string SqlSelectEquipPreset = $@"
        SELECT {BuildQueryField(EquipPresetFields)} FROM ""ddon_equip_preset""
        WHERE ""character_id""=@character_id AND ""job""=@job AND ""preset_no""=@preset_no;";

    private readonly string SqlSelectEquipPresets = $@"
        SELECT {BuildQueryField(EquipPresetFields)} FROM ""ddon_equip_preset""
        WHERE ""character_id""=@character_id AND ""job""=@job
        ORDER BY ""preset_no"" ASC;";

    public override bool InsertEquipPreset(EquipPreset preset, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            return ExecuteNonQuery(conn, SqlInsertEquipPreset, command => AddEquipPresetParams(command, preset)) == 1;
        });
    }

    public override bool UpdateEquipPreset(EquipPreset preset, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            return ExecuteNonQuery(conn, SqlUpdateEquipPreset, command => AddEquipPresetParams(command, preset)) == 1;
        });
    }

    public override bool UpdateEquipPresetName(uint characterId, JobId job, byte presetNo, string presetName, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            return ExecuteNonQuery(conn, SqlUpdateEquipPresetName, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "job", (byte)job);
                AddParameter(command, "preset_no", presetNo);
                AddParameter(command, "preset_name", presetName);
            }) == 1;
        });
    }

    public override bool DeleteEquipPreset(uint characterId, JobId job, byte presetNo, DbConnection? connectionIn = null)
    {
        return ExecuteQuerySafe(connectionIn, conn =>
        {
            return ExecuteNonQuery(conn, SqlDeleteEquipPreset, command =>
            {
                AddParameter(command, "character_id", characterId);
                AddParameter(command, "job", (byte)job);
                AddParameter(command, "preset_no", presetNo);
            }) == 1;
        });
    }

    public override EquipPreset? SelectEquipPreset(uint characterId, JobId job, byte presetNo, DbConnection? connectionIn = null)
    {
        EquipPreset? result = null;

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectEquipPreset,
                command =>
                {
                    AddParameter(command, "character_id", characterId);
                    AddParameter(command, "job", (byte)job);
                    AddParameter(command, "preset_no", presetNo);
                },
                reader =>
                {
                    if (reader.Read())
                    {
                        result = ReadEquipPreset(reader);
                    }
                });
        });

        return result;
    }

    public override List<EquipPreset> SelectEquipPresets(uint characterId, JobId job, DbConnection? connectionIn = null)
    {
        List<EquipPreset> results = new();

        ExecuteQuerySafe(connectionIn, conn =>
        {
            ExecuteReader(conn, SqlSelectEquipPresets,
                command =>
                {
                    AddParameter(command, "character_id", characterId);
                    AddParameter(command, "job", (byte)job);
                },
                reader =>
                {
                    while (reader.Read())
                    {
                        results.Add(ReadEquipPreset(reader));
                    }
                });
        });

        return results;
    }

    private void AddEquipPresetParams(DbCommand command, EquipPreset preset)
    {
        AddParameter(command, "character_id", preset.CharacterId);
        AddParameter(command, "job", (byte)preset.Job);
        AddParameter(command, "preset_no", preset.PresetNo);
        AddParameter(command, "preset_name", preset.PresetName);

        // Performance slots
        AddParameter(command, "p_primary_weapon", preset.PPrimaryWeapon ?? string.Empty);
        AddParameter(command, "p_secondary_weapon", preset.PSecondaryWeapon ?? string.Empty);
        AddParameter(command, "p_head", preset.PHead ?? string.Empty);
        AddParameter(command, "p_body", preset.PBody ?? string.Empty);
        AddParameter(command, "p_clothing", preset.PClothing ?? string.Empty);
        AddParameter(command, "p_arm", preset.PArm ?? string.Empty);
        AddParameter(command, "p_leg", preset.PLeg ?? string.Empty);
        AddParameter(command, "p_leg_wear", preset.PLegWear ?? string.Empty);
        AddParameter(command, "p_over_wear", preset.POverWear ?? string.Empty);
        AddParameter(command, "p_jewelry1", preset.PJewelry1 ?? string.Empty);
        AddParameter(command, "p_jewelry2", preset.PJewelry2 ?? string.Empty);
        AddParameter(command, "p_jewelry3", preset.PJewelry3 ?? string.Empty);
        AddParameter(command, "p_jewelry4", preset.PJewelry4 ?? string.Empty);
        AddParameter(command, "p_jewelry5", preset.PJewelry5 ?? string.Empty);
        AddParameter(command, "p_lantern", preset.PLantern ?? string.Empty);

        // Visual slots
        AddParameter(command, "v_primary_weapon", preset.VPrimaryWeapon ?? string.Empty);
        AddParameter(command, "v_secondary_weapon", preset.VSecondaryWeapon ?? string.Empty);
        AddParameter(command, "v_head", preset.VHead ?? string.Empty);
        AddParameter(command, "v_body", preset.VBody ?? string.Empty);
        AddParameter(command, "v_clothing", preset.VClothing ?? string.Empty);
        AddParameter(command, "v_arm", preset.VArm ?? string.Empty);
        AddParameter(command, "v_leg", preset.VLeg ?? string.Empty);
        AddParameter(command, "v_leg_wear", preset.VLegWear ?? string.Empty);
        AddParameter(command, "v_over_wear", preset.VOverWear ?? string.Empty);
    }

    private EquipPreset ReadEquipPreset(DbDataReader reader)
    {
        EquipPreset preset = new EquipPreset();
        preset.CharacterId = GetUInt32(reader, "character_id");
        preset.Job = (JobId)GetByte(reader, "job");
        preset.PresetNo = GetByte(reader, "preset_no");
        preset.PresetName = GetString(reader, "preset_name");

        // Performance slots
        preset.PPrimaryWeapon = GetStringNullable(reader, reader.GetOrdinal("p_primary_weapon"));
        preset.PSecondaryWeapon = GetStringNullable(reader, reader.GetOrdinal("p_secondary_weapon"));
        preset.PHead = GetStringNullable(reader, reader.GetOrdinal("p_head"));
        preset.PBody = GetStringNullable(reader, reader.GetOrdinal("p_body"));
        preset.PClothing = GetStringNullable(reader, reader.GetOrdinal("p_clothing"));
        preset.PArm = GetStringNullable(reader, reader.GetOrdinal("p_arm"));
        preset.PLeg = GetStringNullable(reader, reader.GetOrdinal("p_leg"));
        preset.PLegWear = GetStringNullable(reader, reader.GetOrdinal("p_leg_wear"));
        preset.POverWear = GetStringNullable(reader, reader.GetOrdinal("p_over_wear"));
        preset.PJewelry1 = GetStringNullable(reader, reader.GetOrdinal("p_jewelry1"));
        preset.PJewelry2 = GetStringNullable(reader, reader.GetOrdinal("p_jewelry2"));
        preset.PJewelry3 = GetStringNullable(reader, reader.GetOrdinal("p_jewelry3"));
        preset.PJewelry4 = GetStringNullable(reader, reader.GetOrdinal("p_jewelry4"));
        preset.PJewelry5 = GetStringNullable(reader, reader.GetOrdinal("p_jewelry5"));
        preset.PLantern = GetStringNullable(reader, reader.GetOrdinal("p_lantern"));

        // Visual slots
        preset.VPrimaryWeapon = GetStringNullable(reader, reader.GetOrdinal("v_primary_weapon"));
        preset.VSecondaryWeapon = GetStringNullable(reader, reader.GetOrdinal("v_secondary_weapon"));
        preset.VHead = GetStringNullable(reader, reader.GetOrdinal("v_head"));
        preset.VBody = GetStringNullable(reader, reader.GetOrdinal("v_body"));
        preset.VClothing = GetStringNullable(reader, reader.GetOrdinal("v_clothing"));
        preset.VArm = GetStringNullable(reader, reader.GetOrdinal("v_arm"));
        preset.VLeg = GetStringNullable(reader, reader.GetOrdinal("v_leg"));
        preset.VLegWear = GetStringNullable(reader, reader.GetOrdinal("v_leg_wear"));
        preset.VOverWear = GetStringNullable(reader, reader.GetOrdinal("v_over_wear"));

        return preset;
    }
}
