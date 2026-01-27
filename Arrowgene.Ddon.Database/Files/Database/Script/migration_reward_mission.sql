-- Migration: Reward Mission (Daily Mission) Progress
-- Tracks daily mission progress and milestone completion per character

CREATE TABLE IF NOT EXISTS "ddon_reward_mission_progress"
(
    "character_id"          INTEGER NOT NULL,
    "mission_id"            INTEGER NOT NULL,
    "current_count"         INTEGER NOT NULL DEFAULT 0,
    "is_complete"           BOOLEAN NOT NULL DEFAULT 0,
    "is_received"           BOOLEAN NOT NULL DEFAULT 0,
    CONSTRAINT "pk_reward_mission_progress" PRIMARY KEY ("character_id", "mission_id"),
    CONSTRAINT "fk_reward_mission_progress_character_id" FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_reward_mission_progress_character_id" ON "ddon_reward_mission_progress" ("character_id");

CREATE TABLE IF NOT EXISTS "ddon_reward_mission_state"
(
    "character_id"          INTEGER PRIMARY KEY NOT NULL,
    "last_reset_time"       DATETIME NOT NULL,
    "missions_completed"    INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT "fk_reward_mission_state_character_id" FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "ddon_reward_mission_milestone"
(
    "character_id"          INTEGER NOT NULL,
    "milestone_id"          INTEGER NOT NULL,
    "is_received"           BOOLEAN NOT NULL DEFAULT 0,
    CONSTRAINT "pk_reward_mission_milestone" PRIMARY KEY ("character_id", "milestone_id"),
    CONSTRAINT "fk_reward_mission_milestone_character_id" FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_reward_mission_milestone_character_id" ON "ddon_reward_mission_milestone" ("character_id");
