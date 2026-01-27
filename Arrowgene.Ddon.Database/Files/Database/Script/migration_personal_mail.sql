-- Migration: Add personal mail (player-to-player) support
-- This is separate from system mail which is admin/server generated

CREATE TABLE IF NOT EXISTS "ddon_personal_mail"
(
    "message_id"            INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    "recipient_character_id" INTEGER                          NOT NULL,
    "sender_character_id"   INTEGER                           NOT NULL,
    "message_state"         INTEGER                           NOT NULL DEFAULT 0,
    "message_title"         VARCHAR(256)                      NOT NULL DEFAULT '',
    "message_body"          VARCHAR(2048)                     NOT NULL DEFAULT '',
    "send_date"             INTEGER                           NOT NULL DEFAULT 0,
    CONSTRAINT "fk_ddon_personal_mail_recipient_id" FOREIGN KEY ("recipient_character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE,
    CONSTRAINT "fk_ddon_personal_mail_sender_id" FOREIGN KEY ("sender_character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_ddon_personal_mail_recipient_id" ON "ddon_personal_mail" ("recipient_character_id");
CREATE INDEX IF NOT EXISTS "idx_ddon_personal_mail_sender_id" ON "ddon_personal_mail" ("sender_character_id");
CREATE INDEX IF NOT EXISTS "idx_ddon_personal_mail_send_date" ON "ddon_personal_mail" ("send_date");
