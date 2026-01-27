-- Migration: Character Courses System
-- Adds tables for character available courses (purchased but not activated) and active courses

-- Character Available Courses table (purchased but not activated)
CREATE TABLE IF NOT EXISTS "ddon_character_course_available"
(
    "id"              INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    "character_id"    INTEGER  NOT NULL,
    "course_id"       INTEGER  NOT NULL,
    "course_name"     TEXT     NOT NULL,
    "duration_sec"    INTEGER  NOT NULL,
    "lineup_id"       INTEGER  NOT NULL,
    "back_icon_id"    INTEGER  NOT NULL DEFAULT 0,
    "frame_icon_id"   INTEGER  NOT NULL DEFAULT 0,
    "purchase_time"   DATETIME NOT NULL,
    CONSTRAINT "fk_character_course_available_character_id" FOREIGN KEY ("character_id")
        REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_character_course_available_character_id"
    ON "ddon_character_course_available" ("character_id");

-- Character Active Courses table (currently active with expiry)
CREATE TABLE IF NOT EXISTS "ddon_character_course_active"
(
    "id"              INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    "character_id"    INTEGER  NOT NULL,
    "course_id"       INTEGER  NOT NULL,
    "course_name"     TEXT     NOT NULL,
    "start_time"      INTEGER  NOT NULL,
    "end_time"        INTEGER  NOT NULL,
    CONSTRAINT "fk_character_course_active_character_id" FOREIGN KEY ("character_id")
        REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_character_course_active_character_id"
    ON "ddon_character_course_active" ("character_id");

CREATE INDEX IF NOT EXISTS "idx_character_course_active_end_time"
    ON "ddon_character_course_active" ("end_time");
