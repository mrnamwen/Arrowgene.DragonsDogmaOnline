-- Migration: Add equipment presets support
-- Stores equipment loadout presets per character and job

CREATE TABLE IF NOT EXISTS "ddon_equip_preset"
(
    "character_id" INTEGER NOT NULL,
    "job"          INTEGER NOT NULL,
    "preset_no"    INTEGER NOT NULL,
    "preset_name"  TEXT    NOT NULL DEFAULT '',
    -- Performance equipment slots (UID references to storage items)
    "p_primary_weapon"   TEXT DEFAULT NULL,
    "p_secondary_weapon" TEXT DEFAULT NULL,
    "p_head"            TEXT DEFAULT NULL,
    "p_body"            TEXT DEFAULT NULL,
    "p_clothing"        TEXT DEFAULT NULL,
    "p_arm"             TEXT DEFAULT NULL,
    "p_leg"             TEXT DEFAULT NULL,
    "p_leg_wear"        TEXT DEFAULT NULL,
    "p_over_wear"       TEXT DEFAULT NULL,
    "p_jewelry1"        TEXT DEFAULT NULL,
    "p_jewelry2"        TEXT DEFAULT NULL,
    "p_jewelry3"        TEXT DEFAULT NULL,
    "p_jewelry4"        TEXT DEFAULT NULL,
    "p_jewelry5"        TEXT DEFAULT NULL,
    "p_lantern"         TEXT DEFAULT NULL,
    -- Visual equipment slots (UID references)
    "v_primary_weapon"   TEXT DEFAULT NULL,
    "v_secondary_weapon" TEXT DEFAULT NULL,
    "v_head"            TEXT DEFAULT NULL,
    "v_body"            TEXT DEFAULT NULL,
    "v_clothing"        TEXT DEFAULT NULL,
    "v_arm"             TEXT DEFAULT NULL,
    "v_leg"             TEXT DEFAULT NULL,
    "v_leg_wear"        TEXT DEFAULT NULL,
    "v_over_wear"       TEXT DEFAULT NULL,
    CONSTRAINT "pk_ddon_equip_preset" PRIMARY KEY ("character_id", "job", "preset_no"),
    CONSTRAINT "fk_ddon_equip_preset_character_id" FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_ddon_equip_preset_character_id" ON "ddon_equip_preset" ("character_id");
