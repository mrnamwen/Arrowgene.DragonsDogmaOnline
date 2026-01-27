-- Mandragora System Tables

-- Table for storing character's owned mandragoras
CREATE TABLE IF NOT EXISTS "ddon_mandragora"
(
    "character_id"      INTEGER NOT NULL,
    "mandragora_id"     INTEGER NOT NULL,
    "name"              TEXT NOT NULL DEFAULT '',
    "species_index"     INTEGER NOT NULL,
    "species_category"  INTEGER NOT NULL,
    "furniture_item_id" INTEGER NOT NULL DEFAULT 0,
    "growth_level"      INTEGER NOT NULL DEFAULT 1,
    CONSTRAINT pk_ddon_mandragora PRIMARY KEY ("character_id", "mandragora_id"),
    CONSTRAINT fk_ddon_mandragora_character_id FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_ddon_mandragora_character_id ON "ddon_mandragora" ("character_id");

-- Table for tracking character's discovered mandragora species
CREATE TABLE IF NOT EXISTS "ddon_mandragora_species_discovery"
(
    "character_id"      INTEGER NOT NULL,
    "species_index"     INTEGER NOT NULL,
    "species_category"  INTEGER NOT NULL,
    "rarity"            INTEGER NOT NULL DEFAULT 2,
    "is_new"            INTEGER NOT NULL DEFAULT 1,
    "discovered_date"   INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT pk_ddon_mandragora_species_discovery PRIMARY KEY ("character_id", "species_index"),
    CONSTRAINT fk_ddon_mandragora_species_discovery_character_id FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_ddon_mandragora_species_discovery_character_id ON "ddon_mandragora_species_discovery" ("character_id");

-- Table for tracking server-wide first discovery of species
CREATE TABLE IF NOT EXISTS "ddon_mandragora_first_discovery"
(
    "species_index"     INTEGER NOT NULL PRIMARY KEY,
    "character_id"      INTEGER NOT NULL,
    "character_name"    TEXT NOT NULL,
    "discovered_date"   INTEGER NOT NULL
);
