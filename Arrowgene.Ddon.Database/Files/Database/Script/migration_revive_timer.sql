-- Migration: Add last_revive_recharge_time to ddon_status_info
-- Fixes: Revival timer exploit (#529) where timer resets across channel changes

ALTER TABLE "ddon_status_info" ADD COLUMN "last_revive_recharge_time" DATETIME DEFAULT NULL;
