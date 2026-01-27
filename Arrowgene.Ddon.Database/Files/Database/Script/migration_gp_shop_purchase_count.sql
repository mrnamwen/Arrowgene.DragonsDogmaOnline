-- Migration: GP Shop Purchase Count Tracking
-- Tracks purchase counts per character per lineup item for enforcing purchase limits

CREATE TABLE IF NOT EXISTS "ddon_gp_shop_purchase_count"
(
    "character_id"    INTEGER NOT NULL,
    "lineup_id"       INTEGER NOT NULL,
    "purchase_count"  INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT "pk_gp_shop_purchase_count" PRIMARY KEY ("character_id", "lineup_id"),
    CONSTRAINT "fk_gp_shop_purchase_count_character_id" FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_gp_shop_purchase_count_character_id" ON "ddon_gp_shop_purchase_count" ("character_id");
