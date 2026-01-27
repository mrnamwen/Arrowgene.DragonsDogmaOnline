-- Migration: Clan Scout Entry and History System
-- Related to clan recruitment features

-- Clan scout entry table for recruitment listings
CREATE TABLE IF NOT EXISTS "ddon_clan_scout_entry" (
    "id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "clan_id" INTEGER NOT NULL,
    "entry_text" TEXT,
    "play_style" INTEGER DEFAULT 0,
    "activity_level" INTEGER DEFAULT 0,
    "min_level" INTEGER DEFAULT 1,
    "recruiting_jobs" TEXT,
    "created_at" DATETIME NOT NULL,
    "updated_at" DATETIME NOT NULL,
    CONSTRAINT "fk_clan_scout_entry_clan" FOREIGN KEY ("clan_id") REFERENCES "ddon_clan_param"("clan_id") ON DELETE CASCADE
);

-- Clan history table for tracking clan events
CREATE TABLE IF NOT EXISTS "ddon_clan_history" (
    "id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "clan_id" INTEGER NOT NULL,
    "event_type" INTEGER NOT NULL,
    "character_id" INTEGER,
    "character_first_name" TEXT,
    "character_last_name" TEXT,
    "details" TEXT,
    "value1" INTEGER DEFAULT 0,
    "value2" INTEGER DEFAULT 0,
    "value3" INTEGER DEFAULT 0,
    "created_at" DATETIME NOT NULL,
    CONSTRAINT "fk_clan_history_clan" FOREIGN KEY ("clan_id") REFERENCES "ddon_clan_param"("clan_id") ON DELETE CASCADE
);

-- Clan join request table for tracking membership applications
CREATE TABLE IF NOT EXISTS "ddon_clan_join_request" (
    "id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "clan_id" INTEGER NOT NULL,
    "character_id" INTEGER NOT NULL,
    "message" TEXT,
    "status" INTEGER DEFAULT 0,
    "created_at" DATETIME NOT NULL,
    "updated_at" DATETIME NOT NULL,
    CONSTRAINT "fk_clan_join_request_clan" FOREIGN KEY ("clan_id") REFERENCES "ddon_clan_param"("clan_id") ON DELETE CASCADE,
    CONSTRAINT "fk_clan_join_request_character" FOREIGN KEY ("character_id") REFERENCES "ddon_character"("character_id") ON DELETE CASCADE
);

-- Clan invite table for tracking clan invitations
CREATE TABLE IF NOT EXISTS "ddon_clan_invite" (
    "id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "clan_id" INTEGER NOT NULL,
    "target_character_id" INTEGER NOT NULL,
    "inviter_character_id" INTEGER NOT NULL,
    "message" TEXT,
    "status" INTEGER DEFAULT 0,
    "created_at" DATETIME NOT NULL,
    "updated_at" DATETIME NOT NULL,
    CONSTRAINT "fk_clan_invite_clan" FOREIGN KEY ("clan_id") REFERENCES "ddon_clan_param"("clan_id") ON DELETE CASCADE,
    CONSTRAINT "fk_clan_invite_target" FOREIGN KEY ("target_character_id") REFERENCES "ddon_character"("character_id") ON DELETE CASCADE,
    CONSTRAINT "fk_clan_invite_inviter" FOREIGN KEY ("inviter_character_id") REFERENCES "ddon_character"("character_id") ON DELETE CASCADE
);

-- Indexes for efficient lookups
CREATE INDEX IF NOT EXISTS "idx_clan_scout_entry_clan_id" ON "ddon_clan_scout_entry"("clan_id");
CREATE INDEX IF NOT EXISTS "idx_clan_history_clan_id" ON "ddon_clan_history"("clan_id");
CREATE INDEX IF NOT EXISTS "idx_clan_history_created_at" ON "ddon_clan_history"("created_at");
CREATE INDEX IF NOT EXISTS "idx_clan_join_request_clan_id" ON "ddon_clan_join_request"("clan_id");
CREATE INDEX IF NOT EXISTS "idx_clan_join_request_character_id" ON "ddon_clan_join_request"("character_id");
CREATE INDEX IF NOT EXISTS "idx_clan_invite_clan_id" ON "ddon_clan_invite"("clan_id");
CREATE INDEX IF NOT EXISTS "idx_clan_invite_target_character_id" ON "ddon_clan_invite"("target_character_id");
