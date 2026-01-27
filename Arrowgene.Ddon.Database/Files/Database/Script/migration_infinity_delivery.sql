-- Migration: Infinity Delivery Progress
-- Tracks infinity delivery progress and border claim history per character

CREATE TABLE IF NOT EXISTS "ddon_infinity_delivery_progress"
(
    "character_id"    INTEGER NOT NULL,
    "category_id"     INTEGER NOT NULL,
    "total_points"    INTEGER NOT NULL DEFAULT 0,
    "items_delivered" INTEGER NOT NULL DEFAULT 0,
    "period_start"    DATETIME NOT NULL,
    CONSTRAINT "pk_infinity_delivery_progress" PRIMARY KEY ("character_id", "category_id"),
    CONSTRAINT "fk_infinity_delivery_progress_character_id" FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_infinity_delivery_progress_character_id" ON "ddon_infinity_delivery_progress" ("character_id");

CREATE TABLE IF NOT EXISTS "ddon_infinity_delivery_border_claim"
(
    "character_id"    INTEGER NOT NULL,
    "border_id"       INTEGER NOT NULL,
    "claimed_at"      DATETIME NOT NULL,
    "period_start"    DATETIME NOT NULL,
    CONSTRAINT "pk_infinity_delivery_border_claim" PRIMARY KEY ("character_id", "border_id", "period_start"),
    CONSTRAINT "fk_infinity_delivery_border_claim_character_id" FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_infinity_delivery_border_claim_character_id" ON "ddon_infinity_delivery_border_claim" ("character_id");
