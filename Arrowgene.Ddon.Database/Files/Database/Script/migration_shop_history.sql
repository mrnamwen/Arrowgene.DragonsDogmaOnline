-- Migration: Shop History System
-- Adds tables for GP purchase history, gacha pull history, and box gacha state

-- GP Purchase History table
CREATE TABLE IF NOT EXISTS "ddon_gp_purchase_history"
(
    "id"              INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    "character_id"    INTEGER                           NOT NULL,
    "shop_type"       INTEGER                           NOT NULL,
    "lineup_id"       INTEGER                           NOT NULL,
    "item_id"         INTEGER                           NOT NULL,
    "quantity"        INTEGER                           NOT NULL,
    "gp_cost"         INTEGER                           NOT NULL,
    "purchase_time"   DATETIME                          NOT NULL,
    CONSTRAINT "fk_gp_purchase_history_character_id" FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_gp_purchase_history_character_id" ON "ddon_gp_purchase_history" ("character_id");
CREATE INDEX IF NOT EXISTS "idx_gp_purchase_history_purchase_time" ON "ddon_gp_purchase_history" ("purchase_time");
CREATE INDEX IF NOT EXISTS "idx_gp_purchase_history_shop_type" ON "ddon_gp_purchase_history" ("shop_type");

-- Gacha Pull History table
CREATE TABLE IF NOT EXISTS "ddon_gacha_pull_history"
(
    "id"              INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    "character_id"    INTEGER                           NOT NULL,
    "gacha_id"        INTEGER                           NOT NULL,
    "pull_type"       INTEGER                           NOT NULL,
    "currency_type"   INTEGER                           NOT NULL,
    "currency_cost"   INTEGER                           NOT NULL,
    "pull_count"      INTEGER                           NOT NULL,
    "pull_time"       DATETIME                          NOT NULL,
    CONSTRAINT "fk_gacha_pull_history_character_id" FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_gacha_pull_history_character_id" ON "ddon_gacha_pull_history" ("character_id");
CREATE INDEX IF NOT EXISTS "idx_gacha_pull_history_gacha_id" ON "ddon_gacha_pull_history" ("gacha_id");
CREATE INDEX IF NOT EXISTS "idx_gacha_pull_history_pull_time" ON "ddon_gacha_pull_history" ("pull_time");

-- Gacha Pull Result table (individual items from a pull)
CREATE TABLE IF NOT EXISTS "ddon_gacha_pull_result"
(
    "id"              INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    "pull_id"         INTEGER                           NOT NULL,
    "result_index"    INTEGER                           NOT NULL,
    "item_id"         INTEGER                           NOT NULL,
    "quantity"        INTEGER                           NOT NULL,
    "rarity"          INTEGER                           NOT NULL,
    "is_bonus"        BOOLEAN                           NOT NULL DEFAULT FALSE,
    CONSTRAINT "fk_gacha_pull_result_pull_id" FOREIGN KEY ("pull_id") REFERENCES "ddon_gacha_pull_history" ("id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_gacha_pull_result_pull_id" ON "ddon_gacha_pull_result" ("pull_id");

-- Box Gacha State table (tracks drawn items from box gacha)
CREATE TABLE IF NOT EXISTS "ddon_box_gacha_state"
(
    "character_id"    INTEGER  NOT NULL,
    "box_gacha_id"    INTEGER  NOT NULL,
    "lineup_id"       INTEGER  NOT NULL,
    "drawn_time"      DATETIME NOT NULL,
    CONSTRAINT "pk_ddon_box_gacha_state" PRIMARY KEY ("character_id", "box_gacha_id", "lineup_id"),
    CONSTRAINT "fk_box_gacha_state_character_id" FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_box_gacha_state_character_box" ON "ddon_box_gacha_state" ("character_id", "box_gacha_id");
